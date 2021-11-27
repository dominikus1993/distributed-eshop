from core.data import CustomerShoppingList, CustomerShoppingListHistoryWriter


class StoreCustomerShoppingListHistoryUseCase:
    __repo: CustomerShoppingListHistoryWriter

    def __init__(self, repo: CustomerShoppingListHistoryWriter) -> None:
        self.__repo = repo

    async def execute(self, evt: CustomerShoppingList):
        await self.__repo.store_changes(evt)