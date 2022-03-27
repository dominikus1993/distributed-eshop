package services

import (
	"context"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/model"
)

type BasketRemoved struct {
	CustomerID int
}

type BasketChanged struct {
	*model.CustomerShoppingList
}

type CustomerBasketChangedEventPublisher interface {
	PublishChanged(context context.Context, changed *BasketChanged) error
}

type CustomerBasketRemovedEventPublisher interface {
	PublishRemoved(context context.Context, rem *BasketRemoved) error
}
