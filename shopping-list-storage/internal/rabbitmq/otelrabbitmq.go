package rabbitmq

import (
	"context"
	"fmt"

	amqp "github.com/rabbitmq/amqp091-go"
	log "github.com/sirupsen/logrus"
	"go.opentelemetry.io/otel"
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
	log.Infoln("Publish")
	span := client.startRabbitMqSpan(ctx, exchangeName, topic, &msg)
	defer client.finishSpan(span)
	err := client.rabbitMqClient.Publish(ctx, exchangeName, topic, msg)
	if err != nil {
		span.RecordError(err)
		span.SetStatus(codes.Error, err.Error())
		return err
	}
	return nil
}

func (client *tracedRabbitMqClient) startRabbitMqSpan(ctx context.Context, exchange, topic string, msg *amqp.Publishing) trace.Span {
	carrier := NewRabbitMqCarrier(msg)
	propagator := otel.GetTextMapPropagator()
	ctx = propagator.Extract(ctx, carrier)
	ctx, span := client.traceProvider.Tracer(defaultTracerName).Start(ctx, "send")
	span.SetAttributes(
		attribute.KeyValue{Key: "messaging.rabbitmq.routing_key", Value: attribute.StringValue(topic)},
		attribute.KeyValue{Key: "messaging.destination", Value: attribute.StringValue(exchange)},
		attribute.KeyValue{Key: "messaging.system", Value: attribute.StringValue("rabbitmq")},
		attribute.KeyValue{Key: "messaging.destination_kind", Value: attribute.StringValue("topic")},
		attribute.KeyValue{Key: "messaging.protocol", Value: attribute.StringValue("AMQP")},
		attribute.KeyValue{Key: "messaging.protocol_version", Value: attribute.StringValue("0.9.1")},
		attribute.KeyValue{Key: "messaging.url", Value: attribute.StringValue(client.connStr)},
	)
	propagator.Inject(ctx, carrier)
	return span
}

func (client *tracedRabbitMqClient) finishSpan(span trace.Span) {
	span.End()
}

type RabbitMqCarrier struct {
	msg *amqp.Publishing
}

// NewConsumerMessageCarrier creates a new ConsumerMessageCarrier.
func NewRabbitMqCarrier(msg *amqp.Publishing) RabbitMqCarrier {
	setHeaderIfEmpty(msg)
	return RabbitMqCarrier{msg: msg}
}

func setHeaderIfEmpty(msg *amqp.Publishing) {
	if msg.Headers == nil {
		msg.Headers = amqp.Table{}
	}
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

func (c RabbitMqCarrier) Keys() []string {
	keys := make([]string, 0, len(c.msg.Headers))
	for k := range c.msg.Headers {
		keys = append(keys, k)
	}
	return keys
}

const defaultTracerName = "rabbitmq"
