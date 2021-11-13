package repositories

import (
	"context"
	"fmt"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/core/model"
	"go.mongodb.org/mongo-driver/bson"
	"go.mongodb.org/mongo-driver/mongo"
	"go.mongodb.org/mongo-driver/mongo/options"
)

type MongoClient struct {
	mongo *mongo.Client
}

func NewClient(connectionString string, ctx context.Context) (*MongoClient, error) {

	// Set client options
	clientOptions := options.Client().ApplyURI(connectionString)

	// connect to MongoDB
	client, err := mongo.Connect(ctx, clientOptions)

	if err != nil {
		return nil, fmt.Errorf("Error when trying connect to mongo: %w", err)
	}

	// Check the connection
	err = client.Ping(ctx, nil)

	if err != nil {
		return nil, fmt.Errorf("Error when trying ping mongo: %w", err)
	}

	return &MongoClient{mongo: client}, nil
}

func (c *MongoClient) Close(ctx context.Context) {
	if err := c.mongo.Disconnect(ctx); err != nil {
		panic(err)
	}
}

type mongoItem struct {
	ItemID       int `bson:"itemId,omitempty" json:"itemId"`
	ItemQuantity int `bson:"itemQuantity,omitempty" json:"itemQuantity"`
}

type mongoCustomerShoppingList struct {
	CustomerID int         `bson:"_id,omitempty" json:"customerId"`
	Items      []mongoItem `bson:"items,omitempty" json:"items"`
}

func newMongoItems(items []model.Item) []mongoItem {
	result := make([]mongoItem, len(items))
	for i, item := range items {
		result[i] = mongoItem{ItemID: item.ItemID, ItemQuantity: item.ItemQuantity}
	}
	return result
}

func toCustomerShoppingList(cs mongoCustomerShoppingList) *model.CustomerShoppingList {
	items := toItems(cs.Items)
	return &model.CustomerShoppingList{CustomerID: cs.CustomerID, Items: items}
}

func toItems(items []mongoItem) []model.Item {
	result := make([]model.Item, len(items))
	for i, item := range items {
		result[i] = model.Item{ItemID: item.ItemID, ItemQuantity: item.ItemQuantity}
	}
	return result
}

func newMongoCustomerShoppingList(cs *model.CustomerShoppingList) *mongoCustomerShoppingList {
	items := newMongoItems(cs.Items)
	return &mongoCustomerShoppingList{CustomerID: cs.CustomerID, Items: items}
}

type mongoShoppingListsRepository struct {
	client *MongoClient
}

func NewMongoShoppingListsRepository(c *MongoClient) *mongoShoppingListsRepository {
	return &mongoShoppingListsRepository{client: c}
}

func (repo *mongoShoppingListsRepository) getCollection() *mongo.Collection {
	return repo.client.mongo.Database("Basket").Collection("customerShoppingLists")
}

func (repo *mongoShoppingListsRepository) Get(ctx context.Context, customerId int) (*model.CustomerShoppingList, error) {
	col := repo.getCollection()
	filter := bson.M{"_id": customerId}
	var res mongoCustomerShoppingList
	if err := col.FindOne(ctx, filter).Decode(&res); err != nil {
		if err == mongo.ErrNoDocuments {
			return nil, nil
		} else {
			return nil, err
		}
	}
	return toCustomerShoppingList(res), nil
}

func (repo *mongoShoppingListsRepository) Remove(ctx context.Context, basket *model.CustomerShoppingList) error {
	col := repo.getCollection()
	filter := bson.M{"_id": basket.CustomerID}
	_, err := col.DeleteOne(ctx, filter)
	if err != nil {
		return fmt.Errorf("error when trying delete customer shopping list, id: %d, Error: %w", basket.CustomerID, err)
	}
	return nil
}

func (repo *mongoShoppingListsRepository) AddOrUpdate(ctx context.Context, basket *model.CustomerShoppingList) error {
	col := repo.getCollection()
	filter := bson.M{"_id": basket.CustomerID}
	opt := options.Update().SetUpsert(true)
	model := newMongoCustomerShoppingList(basket)
	_, err := col.UpdateOne(ctx, filter, model, opt)
	if err != nil {
		return fmt.Errorf("error when trying add or update customer shopping list, id: %d, Error: %w", basket.CustomerID, err)
	}
	return nil
}
