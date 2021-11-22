package repositories

import (
	"context"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/model"
)

type CustomerShoppingListReader interface {
	Get(ctx context.Context, customerId int) (*model.CustomerShoppingList, error)
}

type CustomerShoppingListWriter interface {
	Remove(ctx context.Context, customerId int) error
	AddOrUpdate(ctx context.Context, basket *model.CustomerShoppingList) error
}

type CustomerShoppingListRepository interface {
	CustomerShoppingListReader
	CustomerShoppingListWriter
}
