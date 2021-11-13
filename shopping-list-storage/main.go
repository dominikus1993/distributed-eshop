package main

import (
	"context"
	"log"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/model"
	r "github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/repositories"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/usecase"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/infrastructure/repositories"
	"github.com/gin-gonic/gin"
	tgin "gopkg.in/DataDog/dd-trace-go.v1/contrib/gin-gonic/gin"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

var (
	MongoClient                    *repositories.MongoClient
	GetCustomerShoppingListUseCase usecase.GetCustomerShoppingListUseCase
)

type Customer struct {
	ID int `uri:"id" binding:"required"`
}

func initData(repo r.CustomerShoppingListWriter) error {
	return repo.AddOrUpdate(context.Background(), model.NewCustomerShoppingList(10, []model.Item{{ItemID: 1, ItemQuantity: 332}, {ItemID: 423, ItemQuantity: 323}}))
}

func init() {
	client, err := repositories.NewClient(context.TODO(), "mongodb://127.0.0.1:27017")
	if err != nil {
		log.Fatalln("error when trying connect to mongo")
	}
	MongoClient = client
	repo := repositories.NewMongoShoppingListsRepository(client)
	GetCustomerShoppingListUseCase = *usecase.NewGetCustomerShoppingListUseCase(repo)

	err = initData(repo)
	if err != nil {
		log.Fatalln("error when try init data", err)
	}
}

func main() {
	tracer.Start(
		tracer.WithEnv("local"),
		tracer.WithService("shopping-list-storage"),
		tracer.WithServiceVersion("v1.1.1"),
	)
	defer tracer.Stop()
	defer MongoClient.Close(context.TODO())
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
		res, err := GetCustomerShoppingListUseCase.Execute(c.Request.Context(), customer.ID)
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
	r.Run(":9000") // listen and serve on 0.0.0.0:8080 (for windows "localhost:8080")
}
