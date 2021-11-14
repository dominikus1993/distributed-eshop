package handlers

import (
	"context"

	r "github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/repositories"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/core/usecase"
	"github.com/dominikus1993/distributed-tracing-sample/shopping-list-storage/pkg/infrastructure/repositories"
)

type Api struct {
	mongoClient                       *repositories.MongoClient
	reader                            *r.CustomerShoppingListReader
	writer                            *r.CustomerShoppingListWriter
	getCustomerShoppingListUseCase    *usecase.GetCustomerShoppingListUseCase
	removeCustomerShoppingListUseCase *usecase.RemoveCustomerShoppingListUseCase
	changeCustomerShoppingListUseCase *usecase.ChangeCustomerShoppingListUseCase
}

func Init() *Api {

	return &Api{}
}

func (api *Api) Start(ctx context.Context) {

}

func (api *Api) Close() error {
	return api.Close()
}
