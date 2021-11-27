
from aio_pika.message import IncomingMessage
from core.data import CustomerShoppingList, Item
from core.usecase import StoreCustomerChangedEventUseCase, StoreCustomerRemovedEventUseCase
from ddtrace import tracer
import json

class CustomerBasketChangedHandler:
    __usecase :StoreCustomerChangedEventUseCase
    def __init__(self, usecase :StoreCustomerChangedEventUseCase) -> None:
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

class CustomerBasketRemovedHandler:
    __usecase :StoreCustomerRemovedEventUseCase
    def __init__(self, usecase :StoreCustomerRemovedEventUseCase) -> None:
        self.__usecase = usecase

    @tracer.wrap("rabbitmq_consume", 'shopping-list-analytyst')
    async def handle(self, msg: IncomingMessage): 
        async with msg.process():
            data = json.loads(msg.body.decode('utf-8'))
            await self.__usecase.execute(data["customerId"])