package services

type BasketRemoved struct {
	CustomerId int
}

type BasketChanged struct {
	CustomerId int
}

type CustomerBasketEventPublisher interface {
	PublishRemoved(rem *BasketRemoved) error
	PublishChanged(changed *BasketChanged) error
}
