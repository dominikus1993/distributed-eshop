package usecase

import (
	"context"

	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/core/model"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/core/repositories"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/internal/core/services"
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

type ChangeCustomerShoppingListUseCase struct {
	repo      repositories.CustomerShoppingListWriter
	publisher services.CustomerBasketChangedEventPublisher
}

func NewChangeCustomerShoppingListUseCase(repo repositories.CustomerShoppingListWriter, publisher services.CustomerBasketChangedEventPublisher) *ChangeCustomerShoppingListUseCase {
	return &ChangeCustomerShoppingListUseCase{repo: repo, publisher: publisher}
}

func (uc *ChangeCustomerShoppingListUseCase) Execute(ctx context.Context, basket *model.CustomerShoppingList) error {
	err := uc.repo.AddOrUpdate(ctx, basket)
	if err != nil {
		return err
	}

	return uc.publisher.Publish(&services.BasketChanged{CustomerShoppingList: basket})
}

type RemoveCustomerShoppingListUseCase struct {
	repo repositories.CustomerShoppingListWriter
}

func NewRemoveCustomerShoppingListUseCase(repo repositories.CustomerShoppingListWriter) *RemoveCustomerShoppingListUseCase {
	return &RemoveCustomerShoppingListUseCase{repo: repo}
}

func (uc *RemoveCustomerShoppingListUseCase) Execute(ctx context.Context, customerId int) error {
	return uc.repo.Remove(ctx, customerId)
}
