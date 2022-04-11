package rabbitmq

import (
	"context"
	"fmt"

	amqp "github.com/rabbitmq/amqp091-go"
	"go.opentelemetry.io/otel/attribute"
	"go.opentelemetry.io/otel/codes"
	"go.opentelemetry.io/otel/trace"
)

type tracedRabbitMqClient struct {
	rabbitMqClient
	traceProvider trace.TracerProvider
	connStr       string
}

func NewTracedRabbitMqClient(connStr string, traceProvider trace.TracerProvider) (*tracedRabbitMqClient, error) {
	conn, err := amqp.Dial(connStr)
	if err != nil {
		return nil, err
	}
	ch, err := conn.Channel()
	if err != nil {
		return nil, err
	}
	client := rabbitMqClient{Connection: conn, Channel: ch}
	return &tracedRabbitMqClient{rabbitMqClient: client, connStr: connStr, traceProvider: traceProvider}, nil
}

func (client *tracedRabbitMqClient) Publish(ctx context.Context, exchangeName string, topic string, msg amqp.Publishing) error {
	var span trace.Span
	ctx, span = client.traceProvider.Tracer("rabbitmq").Start(ctx, "send")
	defer span.End()
	span.SetAttributes(
		attribute.KeyValue{Key: "messaging.rabbitmq.routing_key", Value: attribute.StringValue(topic)},
		attribute.KeyValue{Key: "messaging.destination", Value: attribute.StringValue(exchangeName)},
		attribute.KeyValue{Key: "messaging.system", Value: attribute.StringValue("rabbitmq")},
		attribute.KeyValue{Key: "messaging.destination_kind", Value: attribute.StringValue("topic")},
		attribute.KeyValue{Key: "messaging.protocol", Value: attribute.StringValue("AMQP")},
		attribute.KeyValue{Key: "messaging.protocol_version", Value: attribute.StringValue("0.9.1")},
		attribute.KeyValue{Key: "messaging.url", Value: attribute.StringValue(client.connStr)},
	)
	newMsg := msg
	carrier := NewRabbitMqCarrier(&newMsg)
	c := propagation.ExtractHTTP(context.Background(), cfg.Propagators, carrier)
	newMsg.Headers = headers
	err := client.rabbitMqClient.Publish(ctx, exchangeName, topic, newMsg)
	if err != nil {
		span.RecordError(err)
		span.SetStatus(codes.Error, err.Error())
		return err
	}
	return nil
}

type RabbitMqCarrier struct {
	msg *amqp.Publishing
}

// NewConsumerMessageCarrier creates a new ConsumerMessageCarrier.
func NewRabbitMqCarrier(msg *amqp.Publishing) RabbitMqCarrier {
	return RabbitMqCarrier{msg: msg}
}

// Get retrieves a single value for a given key.
func (c RabbitMqCarrier) Get(key string) string {
	for k, value := range c.msg.Headers {
		if string(k) == key {
			return fmt.Sprint(value)
		}
	}
	return ""
}

// Set sets a header.
func (c RabbitMqCarrier) Set(key, val string) {
	// Ensure uniqueness of keys
	for k := range c.msg.Headers {
		if k == key {
			delete(c.msg.Headers, k)
		}
	}
	c.msg.Headers[key] = val
}
