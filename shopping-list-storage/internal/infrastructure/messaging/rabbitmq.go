package messaging

import (
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/core/services"
	amqp "github.com/rabbitmq/amqp091-go"
	log "github.com/sirupsen/logrus"
)

type RabbitMqClient struct {
	Connection *amqp.Connection
	Channel    *amqp.Channel
}

func NewRabbitMqClient(connStr string) (*RabbitMqClient, error) {
	conn, err := amqp.Dial(connStr)
	if err != nil {
		return nil, err
	}
	ch, err := conn.Channel()
	if err != nil {
		return nil, err
	}
	return &RabbitMqClient{Connection: conn, Channel: ch}, nil
}

func (client *RabbitMqClient) Close() {
	err := client.Channel.Close()
	if err != nil {
		log.WithError(err).Fatalln("channel close error")
	}
	err = client.Connection.Close()
	if err != nil {
		log.WithError(err).Fatalln("client connection close error")
	}
}

type RabbitmMqCustomerBasketChangedEventPublisher struct {
}

func NewRabbitmMqCustomerBasketChangedEventPublisher() *RabbitmMqCustomerBasketChangedEventPublisher {
	return &RabbitmMqCustomerBasketChangedEventPublisher{}
}

func (p *RabbitmMqCustomerBasketChangedEventPublisher) Publish(changed *services.BasketChanged) error {
	return nil
}
