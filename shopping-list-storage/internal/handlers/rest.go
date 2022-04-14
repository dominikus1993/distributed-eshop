package handlers

import (
	"context"
	"fmt"
	"net/http"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/env"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/mongodb"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/rabbitmq"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/model"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/repositories"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/usecase"
	"github.com/gin-gonic/gin"
	ginlogrus "github.com/toorop/gin-logrus"
	"go.opentelemetry.io/contrib/instrumentation/github.com/gin-gonic/gin/otelgin"
	"go.opentelemetry.io/otel"
)

type Customer struct {
	ID int `uri:"id" binding:"required"`
}

type ItemRequest struct {
	ItemID       int `form:"itemId" json:"itemId" xml:"itemId"  binding:"required"`
	ItemQuantity int `form:"itemQuantity" json:"itemQuantity" xml:"itemQuantity" binding:"required"`
}

type ChangeCustomerShoppingListRequest struct {
	Items []ItemRequest `form:"items" json:"items" xml:"items" binding:"required"`
}

type Api struct {
	mongoClient                       *mongodb.MongoClient
	getCustomerShoppingListUseCase    *usecase.GetCustomerShoppingListUseCase
	removeCustomerShoppingListUseCase *usecase.RemoveCustomerShoppingListUseCase
	changeCustomerShoppingListUseCase *usecase.ChangeCustomerShoppingListUseCase
	rabbitMq                          rabbitmq.RabbitMqClient
	shudownTracing                    func()
}

func initData(repo repositories.CustomerShoppingListWriter) error {
	return repo.AddOrUpdate(context.Background(), model.NewCustomerShoppingList(10, []model.Item{{ItemID: 1, ItemQuantity: 332}, {ItemID: 423, ItemQuantity: 323}}))
}

func InitApi(ctx context.Context) (*Api, error) {
	shoutdown := initPrivder(ctx)

	client, err := mongodb.NewTracedClient(ctx, env.GetEnvOrDefault("MONGODB_CONNECTION", "mongodb://db:27017"), otel.GetTracerProvider())
	if err != nil {
		return nil, fmt.Errorf("error when trying connect to mongo, ERR: %w", err)
	}
	repo := mongodb.NewMongoShoppingListsRepository(client)
	rabbitmqClient, err := rabbitmq.NewTracedRabbitMqClient(env.GetEnvOrDefault("RABBITMQ_CONNECTION", "amqp://guest:guest@rabbitmq:5672/"), otel.GetTracerProvider())
	if err != nil {
		return nil, fmt.Errorf("error when trying connect to rabbitmq, ERR: %w", err)
	}
	publisher := rabbitmq.NewRabbitmMqCustomerBasketChangedEventPublisher(rabbitmqClient, &rabbitmq.RabbitMqConfig{ExchangeName: "basket", Topic: "changed"}, &rabbitmq.RabbitMqConfig{ExchangeName: "basket", Topic: "removed"})
	getCustomerShoppingListUseCase := usecase.NewGetCustomerShoppingListUseCase(repo)
	changeCustomerShoppingListUseCase := usecase.NewChangeCustomerShoppingListUseCase(repo, publisher)
	removeCustomerShoppingListUseCase := usecase.NewRemoveCustomerShoppingListUseCase(repo, publisher)
	err = initData(repo)
	if err != nil {
		return nil, fmt.Errorf("error when trying save initial data, ERR: %w", err)
	}
	return &Api{mongoClient: client, removeCustomerShoppingListUseCase: removeCustomerShoppingListUseCase, getCustomerShoppingListUseCase: getCustomerShoppingListUseCase, changeCustomerShoppingListUseCase: changeCustomerShoppingListUseCase, rabbitMq: rabbitmqClient, shudownTracing: shoutdown}, nil
}

func (api *Api) Start(ctx context.Context) error {
	r := gin.New()
	log := initLogger()
	r.Use(ginlogrus.Logger(log), gin.Recovery())
	r.Use(otelgin.Middleware("shopping.list.storage", otelgin.WithTracerProvider(otel.GetTracerProvider())))
	r.GET("/ping", func(c *gin.Context) {
		c.JSON(200, gin.H{
			"message": "pong",
		})
	})

	r.GET("/shoppingLists/:id", func(c *gin.Context) {
		var customer Customer
		if err := c.ShouldBindUri(&customer); err != nil {
			c.JSON(400, gin.H{"msg": err.Error()})
			return
		}
		res, err := api.getCustomerShoppingListUseCase.Execute(c.Request.Context(), customer.ID)
		if err != nil {
			c.Error(err)
			c.Status(500)
			return
		}
		if res == nil {
			c.Status(404)
			return
		}
		items := make([]struct {
			ItemID       int `json:"itemId"`
			ItemQuantity int `json:"ItemQuantity"`
		}, len(res.Items))
		for i, item := range res.Items {
			items[i] = struct {
				ItemID       int `json:"itemId"`
				ItemQuantity int `json:"ItemQuantity"`
			}{ItemID: item.ItemID, ItemQuantity: item.ItemQuantity}
		}
		c.JSON(200, gin.H{
			"customerId": res.CustomerID,
			"items":      items,
		})
	})

	r.DELETE("/shoppingLists/:id", func(c *gin.Context) {
		var customer Customer
		if err := c.ShouldBindUri(&customer); err != nil {
			c.JSON(400, gin.H{"msg": err.Error()})
			return
		}
		err := api.removeCustomerShoppingListUseCase.Execute(c.Request.Context(), customer.ID)
		if err != nil {
			c.Error(err)
			c.Status(500)
			return
		}
		c.Status(204)
	})

	r.POST("/shoppingLists/:id", func(c *gin.Context) {
		var customer Customer
		if err := c.ShouldBindUri(&customer); err != nil {
			c.JSON(400, gin.H{"msg": err.Error()})
			return
		}
		var json ChangeCustomerShoppingListRequest
		if err := c.ShouldBindJSON(&json); err != nil {
			c.JSON(http.StatusBadRequest, gin.H{"error": err.Error()})
			return
		}
		items := make([]model.Item, len(json.Items))
		for i, item := range json.Items {
			items[i] = *model.NewItem(item.ItemID, item.ItemQuantity)
		}
		model := model.NewCustomerShoppingList(customer.ID, items)
		err := api.changeCustomerShoppingListUseCase.Execute(c.Request.Context(), model)
		if err != nil {
			c.Error(err)
			c.Status(500)
			return
		}
		c.Status(201)
	})
	return r.Run(":9000") // listen and serve on 0.0.0.0:8080 (for windows "localhost:8080")
}

func (api *Api) Close(ctx context.Context) {
	api.mongoClient.Close(ctx)
	api.rabbitMq.Close()
	api.shudownTracing()
}
