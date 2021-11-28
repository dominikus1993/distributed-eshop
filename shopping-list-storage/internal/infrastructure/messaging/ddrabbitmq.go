package messaging

import (
	"context"
	"fmt"

	amqp "github.com/rabbitmq/amqp091-go"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

type tracedRabbitMqClient struct {
	rabbitMqClient
}

func NewTracedRabbitMqClient(connStr string) (*tracedRabbitMqClient, error) {
	conn, err := amqp.Dial(connStr)
	if err != nil {
		return nil, err
	}
	ch, err := conn.Channel()
	if err != nil {
		return nil, err
	}
	client := rabbitMqClient{Connection: conn, Channel: ch}
	return &tracedRabbitMqClient{rabbitMqClient: client}, nil
}

func (client *tracedRabbitMqClient) Publish(ctx context.Context, exchangeName string, topic string, msg amqp.Publishing) error {
	span, ctx := tracer.StartSpanFromContext(ctx, "rabbitmq.publish", tracer.ServiceName("shopping-list-storage-api"))
	defer span.Finish()
	span.SetTag("exchange_name", exchangeName)
	span.SetTag("topic", topic)
	spanId := span.Context().SpanID()
	traceId := span.Context().TraceID()
	newMsg := msg
	var headers map[string]interface{} = map[string]interface{}{"x-datadog-trace-id": fmt.Sprintf("%d", traceId), "x-datadog-parent-id": fmt.Sprintf("%d", spanId)}
	newMsg.Headers = headers
	return client.rabbitMqClient.Publish(ctx, exchangeName, topic, newMsg)
}
