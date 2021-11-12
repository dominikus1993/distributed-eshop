package main

import (
	"github.com/gin-gonic/gin"
	tgin "gopkg.in/DataDog/dd-trace-go.v1/contrib/gin-gonic/gin"
	"gopkg.in/DataDog/dd-trace-go.v1/ddtrace/tracer"
)

type Customer struct {
	ID int `uri:"id" binding:"required"`
}

func main() {
	tracer.Start(
		tracer.WithEnv("local"),
		tracer.WithService("chat-message-storage"),
		tracer.WithServiceVersion("v1.1.1"),
	)
	defer tracer.Stop()
	tgin.WithAnalytics(true)
	r := gin.New()
	r.Use(tgin.Middleware("chat-message-storage"))
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
		var items []struct {
			ItemID       int `json:"itemId"`
			ItemQuantity int `json:"ItemQuantity"`
		} = []struct {
			ItemID       int `json:"itemId"`
			ItemQuantity int `json:"ItemQuantity"`
		}{{ItemID: 2, ItemQuantity: 2}}
		c.JSON(200, gin.H{
			"customerId": customer.ID,
			"items":      items,
		})
	})
	r.Run(":9000") // listen and serve on 0.0.0.0:8080 (for windows "localhost:8080")
}
