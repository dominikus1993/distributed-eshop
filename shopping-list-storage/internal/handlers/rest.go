package handlers

import (
	"context"
	"fmt"
	"net/http"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/common"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/core/model"
	r "github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/core/repositories"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/core/usecase"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/infrastructure/messaging"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/infrastructure/repositories"
	"github.com/gin-gonic/gin"
	tgin "gopkg.in/DataDog/dd-trace-go.v1/contrib/gin-gonic/gin"
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
	mongoClient                       *repositories.MongoClient
	getCustomerShoppingListUseCase    *usecase.GetCustomerShoppingListUseCase
	removeCustomerShoppingListUseCase *usecase.RemoveCustomerShoppingListUseCase
	changeCustomerShoppingListUseCase *usecase.ChangeCustomerShoppingListUseCase
}

func initData(repo r.CustomerShoppingListWriter) error {
	return repo.AddOrUpdate(context.Background(), model.NewCustomerShoppingList(10, []model.Item{{ItemID: 1, ItemQuantity: 332}, {ItemID: 423, ItemQuantity: 323}}))
}

func InitApi() (*Api, error) {
	client, err := repositories.NewTracedClient(context.TODO(), common.GetEnvOrDefault("MONGODB_CONNECTION", "mongodb://127.0.0.1:27017"))
	if err != nil {
		return nil, fmt.Errorf("error when trying connect to mongo, ERR: %w", err)
	}
	repo := repositories.NewMongoShoppingListsRepository(client)
	publisher := messaging.NewRabbitmMqCustomerBasketChangedEventPublisher()
	getCustomerShoppingListUseCase := usecase.NewGetCustomerShoppingListUseCase(repo)
	changeCustomerShoppingListUseCase := usecase.NewChangeCustomerShoppingListUseCase(repo, publisher)
	removeCustomerShoppingListUseCase := usecase.NewRemoveCustomerShoppingListUseCase(repo)
	err = initData(repo)
	if err != nil {
		return nil, fmt.Errorf("error when trying save initial data, ERR: %w", err)
	}
	return &Api{mongoClient: client, removeCustomerShoppingListUseCase: removeCustomerShoppingListUseCase, getCustomerShoppingListUseCase: getCustomerShoppingListUseCase, changeCustomerShoppingListUseCase: changeCustomerShoppingListUseCase}, nil
}

func (api *Api) Start(ctx context.Context) error {
	tgin.WithAnalytics(true)
	r := gin.New()
	r.Use(gin.Logger(), gin.Recovery())
	r.Use(tgin.Middleware("shopping-list-storage"))

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
}
