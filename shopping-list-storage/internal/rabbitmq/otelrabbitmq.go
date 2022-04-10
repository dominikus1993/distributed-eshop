package rabbitmq

import (
	"context"

	amqp "github.com/rabbitmq/amqp091-go"
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
	var span trace.Span
	ctx, span = otel.Tracer("rabbitmq").Start(ctx, "send")
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
	spanId := span.SpanContext().SpanID()
	newMsg := msg
	var headers = map[string]interface{}{"span_id": spanId.String()}
	newMsg.Headers = headers
	err := client.rabbitMqClient.Publish(ctx, exchangeName, topic, newMsg)
	if err != nil {
		span.RecordError(err)
		span.SetStatus(codes.Error, err.Error())
		return err
	}
	return nil
}
