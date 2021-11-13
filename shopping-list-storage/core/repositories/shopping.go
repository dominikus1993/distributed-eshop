package repositories

import (
	"context"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/core/model"
)

type CustomerShoppingListRepository interface {
	Get(ctx context.Context, customerId int) (*model.CustomerShoppingList, error)
	Remove(ctx context.Context, basket *model.CustomerShoppingList) error
	AddOrUpdate(ctx context.Context, basket *model.CustomerShoppingList) error
}
