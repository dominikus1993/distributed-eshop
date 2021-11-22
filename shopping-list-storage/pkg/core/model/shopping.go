package model

type Item struct {
	ItemID       int
	ItemQuantity int
}

func NewItem(itemID, itemQuantity int) *Item {
	return &Item{ItemID: itemID, ItemQuantity: itemQuantity}
}

type CustomerShoppingList struct {
	CustomerID int
	Items      []Item
}

func NewCustomerShoppingList(customerId int, Items []Item) *CustomerShoppingList {
	return &CustomerShoppingList{CustomerID: customerId, Items: Items}
}
