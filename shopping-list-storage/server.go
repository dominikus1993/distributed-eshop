package main

import (
	"context"
	"errors"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/shoppinglist"
)

type fakeCustomerShoppingListServer struct {
}

func NewFakeCustomerShoppingListServer() *fakeCustomerShoppingListServer {
	return &fakeCustomerShoppingListServer{}
}

func (s *fakeCustomerShoppingListServer) GetCustomerShoppingList(ctx context.Context, req *shoppinglist.GetCustomerShoppingListRequest) (*shoppinglist.CustomerShoppingList, error) {
	if req.CustomerId == 2 {
		return nil, errors.New("customer not found")
	}
	return &shoppinglist.CustomerShoppingList{CustomerId: req.CustomerId, Items: []*shoppinglist.CustomerShoppingList_Item{{ItemId: 1, ItemQuantity: 10}}}, nil
}
