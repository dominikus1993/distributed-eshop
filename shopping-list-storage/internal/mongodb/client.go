package mongodb

import (
	"context"
	"fmt"

	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
	mongotrace "gopkg.in/DataDog/dd-trace-go.v1/contrib/go.mongodb.org/mongo-driver/mongo"
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

func NewTracedClient(ctx context.Context, connectionString string) (*MongoClient, error) {

	// Set client options
	clientOptions := options.Client().ApplyURI(connectionString)
	clientOptions.Monitor = mongotrace.NewMonitor(mongotrace.WithServiceName("shopping-list-storage-api"), mongotrace.WithAnalytics(true))

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
