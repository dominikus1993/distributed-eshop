package messaging

import (
	"context"
	"encoding/json"
	"fmt"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/core/services"
	amqp "github.com/rabbitmq/amqp091-go"
	log "github.com/sirupsen/logrus"
)

type RabbitMqClient interface {
	GetConnection() *amqp.Connection
	GetChannel() *amqp.Channel
	Close()
	DeclareExchange(exchangeName string) error
	Publish(ctx context.Context, exchangeName string, topic string, msg interface{}) error
}

type rabbitMqClient struct {
	Connection *amqp.Connection
	Channel    *amqp.Channel
}

type RabbitMqConfig struct {
	ExchangeName string
	Topic        string
}

func NewRabbitMqClient(connStr string) (*rabbitMqClient, error) {
	conn, err := amqp.Dial(connStr)
	if err != nil {
		return nil, err
	}
	ch, err := conn.Channel()
	if err != nil {
		return nil, err
	}
	return &rabbitMqClient{Connection: conn, Channel: ch}, nil
}

func (client *rabbitMqClient) GetChannel() *amqp.Channel {
	return client.Channel
}

func (client *rabbitMqClient) GetConnection() *amqp.Connection {
	return client.Connection
}

func (client *rabbitMqClient) Close() {
	err := client.Channel.Close()
	if err != nil {
		log.WithError(err).Fatalln("channel close error")
	}
	err = client.Connection.Close()
	if err != nil {
		log.WithError(err).Fatalln("client connection close error")
	}
}

func (client *rabbitMqClient) DeclareExchange(exchangeName string) error {
	return client.Channel.ExchangeDeclare(
		exchangeName, // name
		"topic",      // type
		true,         // durable
		false,        // auto-deleted
		false,        // internal
		false,        // no-wait
		nil,          // arguments
	)
}

func (client *rabbitMqClient) Publish(ctx context.Context, exchangeName string, topic string, msg interface{}) error {
	jsonBody, err := json.Marshal(msg)
	if err != nil {
		return fmt.Errorf("marshal json error, %w", err)
	}
	return client.Channel.Publish(exchangeName, log.ErrorKey, false, false, amqp.Publishing{ContentType: "application/json", Body: jsonBody})
}

type RabbitmMqCustomerBasketEventPublisher struct {
	customerBsketChangedConfig *RabbitMqConfig
	customerBsketRemovedConfig *RabbitMqConfig
	rabbitmq                   RabbitMqClient
}

func NewRabbitmMqCustomerBasketChangedEventPublisher(rabbitmq RabbitMqClient, customerBsketChangedConfig *RabbitMqConfig, customerBsketRemovedConfig *RabbitMqConfig) *RabbitmMqCustomerBasketEventPublisher {
	return &RabbitmMqCustomerBasketEventPublisher{rabbitmq: rabbitmq, customerBsketChangedConfig: customerBsketChangedConfig, customerBsketRemovedConfig: customerBsketRemovedConfig}
}

func (p *RabbitmMqCustomerBasketEventPublisher) PublishChanged(context context.Context, changed *services.BasketChanged) error {
	err := p.rabbitmq.DeclareExchange(p.customerBsketChangedConfig.ExchangeName)
	if err != nil {
		return fmt.Errorf("error when publish message to rabbitmq %w", err)
	}

	items := make([]struct {
		ItemID       int `json:"itemId"`
		ItemQuantity int `json:"itemQuantity"`
	}, len(changed.Items))

	for i, item := range changed.Items {
		items[i] = struct {
			ItemID       int `json:"itemId"`
			ItemQuantity int `json:"itemQuantity"`
		}{ItemID: item.ItemID, ItemQuantity: item.ItemQuantity}
	}

	jsonMsg := struct {
		CustomerId int `json:"customerId"`
		Items      []struct {
			ItemID       int `json:"itemId"`
			ItemQuantity int `json:"itemQuantity"`
		} `json:"items"`
	}{
		CustomerId: changed.CustomerID,
		Items:      items,
	}

	return p.rabbitmq.Publish(context, p.customerBsketChangedConfig.ExchangeName, p.customerBsketChangedConfig.Topic, jsonMsg)
}

func (p *RabbitmMqCustomerBasketEventPublisher) PublishRemoved(context context.Context, rem *services.BasketRemoved) error {
	err := p.rabbitmq.DeclareExchange(p.customerBsketChangedConfig.ExchangeName)
	if err != nil {
		return fmt.Errorf("error when publish message to rabbitmq %w", err)
	}
	jsonMsg := struct {
		CustomerId int `json:"customerId"`
	}{
		CustomerId: rem.CustomerID,
	}

	return p.rabbitmq.Publish(context, p.customerBsketRemovedConfig.ExchangeName, p.customerBsketRemovedConfig.Topic, jsonMsg)
}
