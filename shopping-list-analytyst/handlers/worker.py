
from aio_pika.message import IncomingMessage
from core.data import CustomerShoppingList, Item
from core.usecase import StoreCustomerShoppingListHistoryUseCase
from ddtrace import tracer
import json

class CustomerBasketChangedHandler:
    __usecase :StoreCustomerShoppingListHistoryUseCase
    def __init__(self, usecase :StoreCustomerShoppingListHistoryUseCase) -> None:
        self.__usecase = usecase

    @tracer.wrap("rabbitmq_consume", 'shopping-list-analytyst')
    async def handle(self, msg: IncomingMessage): 
        async with msg.process():
            print("[x] %r" % msg.body.decode('utf-8'))
            data = json.loads(msg.body.decode('utf-8'))
            items = []
            for item in data["items"]:
                items.append(Item(item["itemId"], item["itemQuantity"]))
            evt = CustomerShoppingList(data["customerId"], items)
            print("xD", evt)
            await self.__usecase.execute(evt)