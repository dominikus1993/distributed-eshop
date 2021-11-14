package handlers

import (
	"context"
	"fmt"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/model"
	r "github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/repositories"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/usecase"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/infrastructure/repositories"
	"github.com/gin-gonic/gin"
	tgin "gopkg.in/DataDog/dd-trace-go.v1/contrib/gin-gonic/gin"
)

type Customer struct {
	ID int `uri:"id" binding:"required"`
}

type Api struct {
	mongoClient                       *repositories.MongoClient
	reader                            *r.CustomerShoppingListReader
	writer                            *r.CustomerShoppingListWriter
	getCustomerShoppingListUseCase    *usecase.GetCustomerShoppingListUseCase
	removeCustomerShoppingListUseCase *usecase.RemoveCustomerShoppingListUseCase
	changeCustomerShoppingListUseCase *usecase.ChangeCustomerShoppingListUseCase
}

func initData(repo r.CustomerShoppingListWriter) error {
	return repo.AddOrUpdate(context.Background(), model.NewCustomerShoppingList(10, []model.Item{{ItemID: 1, ItemQuantity: 332}, {ItemID: 423, ItemQuantity: 323}}))
}

func Init() (*Api, error) {
	client, err := repositories.NewTracedClient(context.TODO(), "mongodb://127.0.0.1:27017")
	if err != nil {
		return nil, fmt.Errorf("error when trying connect to mongo, ERR: %w", err)
	}
	repo := repositories.NewMongoShoppingListsRepository(client)
	getCustomerShoppingListUseCase := usecase.NewGetCustomerShoppingListUseCase(repo)

	err = initData(repo)
	if err != nil {
		return nil, fmt.Errorf("error when trying save initial data, ERR: %w", err)
	}
	return &Api{mongoClient: client, getCustomerShoppingListUseCase: getCustomerShoppingListUseCase}, nil
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
	return r.Run(":9000") // listen and serve on 0.0.0.0:8080 (for windows "localhost:8080")
}

func (api *Api) Close(ctx context.Context) {
	api.mongoClient.Close(ctx)
}
