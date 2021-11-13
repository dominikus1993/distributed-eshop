package usecase

import (
	"context"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/model"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/repositories"
)

type GetCustomerShoppingListUseCase struct {
	repo repositories.CustomerShoppingListReader
}

func NewGetCustomerShoppingListUseCase(repo repositories.CustomerShoppingListReader) *GetCustomerShoppingListUseCase {
	return &GetCustomerShoppingListUseCase{repo: repo}
}

func (uc *GetCustomerShoppingListUseCase) Execute(ctx context.Context, customerId int) (*model.CustomerShoppingList, error) {
	result, err := uc.repo.Get(ctx, customerId)
	if err != nil {
		return nil, err
	}
	return result, nil
}
