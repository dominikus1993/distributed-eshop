from abc import ABC, abstractmethod
from dataclasses import dataclass
from datetime import datetime
from typing import AsyncIterable, Callable, Coroutine

@dataclass
class Item:
    item_quantity: int
    item_id: int

@dataclass
class CustomerShoppingList:
    customer_id: int
    items: list[Item]

class CustomerShoppingListHistoryWriter(ABC):
    @abstractmethod
    def store_changes(self, evt: CustomerShoppingList) -> Coroutine:
        pass

    @abstractmethod
    def store_deletion(self, customer_id: int) -> Coroutine:
        pass

@dataclass
class CustomerShoppingListChanged:
    event_type = "CustomerShoppingListChanged"
    customer_id: int
    items: list[Item]
    changed_at: datetime

@dataclass
class CustomerShoppingListRemoved:
    event_type = "CustomerShoppingListRemoved"
    customer_id: int
    removed_at: datetime

class CustomerShoppingListHistoryReader(ABC):
    @abstractmethod
    def read(self, customer_id: int) -> AsyncIterable[CustomerShoppingListChanged | CustomerShoppingListRemoved]:
        pass