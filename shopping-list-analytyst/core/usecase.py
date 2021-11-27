from core.data import CustomerShoppingList, CustomerShoppingListHistoryWriter


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
