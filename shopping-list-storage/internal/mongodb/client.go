package mongodb

import (
	"context"
	"fmt"

	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	"go.opentelemetry.io/contrib/instrumentation/go.mongodb.org/mongo-driver/mongo/otelmongo"
	"go.opentelemetry.io/otel/trace"
)

type MongoClient struct {
	mongo *mongo.Client
}

func NewClient(ctx context.Context, connectionString string) (*MongoClient, error) {

	// Set client options
	clientOptions := options.Client().ApplyURI(connectionString)

	// connect to MongoDB
	client, err := mongo.Connect(ctx, clientOptions)

	if err != nil {
		return nil, fmt.Errorf("error when trying connect to mongo: %w", err)
	}

	// Check the connection
	err = client.Ping(ctx, nil)

	if err != nil {
		return nil, fmt.Errorf("error when trying ping mongo: %w", err)
	}

	return &MongoClient{mongo: client}, nil
}

func NewTracedClient(ctx context.Context, connectionString string, provider trace.TracerProvider) (*MongoClient, error) {

	// Set client options
	clientOptions := options.Client().ApplyURI(connectionString)
	clientOptions.Monitor = otelmongo.NewMonitor(otelmongo.WithTracerProvider(provider))

	// connect to MongoDB
	client, err := mongo.Connect(ctx, clientOptions)

	if err != nil {
		return nil, fmt.Errorf("error when trying connect to mongo: %w", err)
	}

	// Check the connection
	err = client.Ping(ctx, nil)

	if err != nil {
		return nil, fmt.Errorf("error when trying ping mongo: %w", err)
	}

	return &MongoClient{mongo: client}, nil
}

func (c *MongoClient) Close(ctx context.Context) {
	if err := c.mongo.Disconnect(ctx); err != nil {
		panic(err)
	}
}
