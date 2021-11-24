package services

import "github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/core/model"

type BasketRemoved struct {
	CustomerId int
}

type BasketChanged struct {
	*model.CustomerShoppingList
}

type CustomerBasketChangedEventPublisher interface {
	Publish(changed *BasketChanged) error
}

type CustomerBasketRemovedEventPublisher interface {
	Publish(rem *BasketRemoved) error
}
