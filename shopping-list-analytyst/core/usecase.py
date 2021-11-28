from dataclasses import dataclass
from datetime import datetime
from core.data import CustomerShoppingList, CustomerShoppingListHistoryReader, CustomerShoppingListHistoryWriter, CustomerShoppingListRemoved, Item


class StoreCustomerChangedEventUseCase:
    __repo: CustomerShoppingListHistoryWriter

    def __init__(self, repo: CustomerShoppingListHistoryWriter) -> None:
        self.__repo = repo

    async def execute(self, evt: CustomerShoppingList):
        await self.__repo.store_changes(evt)

class StoreCustomerRemovedEventUseCase:
    __repo: CustomerShoppingListHistoryWriter

    def __init__(self, repo: CustomerShoppingListHistoryWriter) -> None:
        self.__repo = repo

    async def execute(self, customer_id: int):
        await self.__repo.store_deletion(customer_id)

@dataclass
class CustomerShoppingListLogDto:
    event_type: str
    customer_id: int
    items: list[Item] | None
    date: str


class ReadCustomerShoppingListHistoryUseCase:
    __repo: CustomerShoppingListHistoryReader

    def __init__(self, repo: CustomerShoppingListHistoryReader) -> None:
        self.__repo = repo

    async def execute(self, customer_id: int):
        async for evt in self.__repo.read(customer_id):
            if isinstance(evt, CustomerShoppingListRemoved):
                yield  CustomerShoppingListLogDto(evt.event_type, evt.customer_id, None, evt.removed_at.isoformat())
            else:
                yield  CustomerShoppingListLogDto(evt.event_type, evt.customer_id, evt.items, evt.changed_at.isoformat()) 