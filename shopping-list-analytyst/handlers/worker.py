from aio_pika.message import IncomingMessage
from core.data import CustomerShoppingList, Item
from core.usecase import StoreCustomerChangedEventUseCase, StoreCustomerRemovedEventUseCase
import json
from opentelemetry import trace
from opentelemetry.sdk.trace import TracerProvider

class CustomerBasketChangedHandler:
    __usecase :StoreCustomerChangedEventUseCase
    __provider: TracerProvider
    def __init__(self, usecase :StoreCustomerChangedEventUseCase, tp: TracerProvider) -> None:
        self.__usecase = usecase
        self.__provider = tp

    async def handle(self, msg: IncomingMessage): 
        async with msg.process():
            trace.get_tracer(__name__).start_span(name="handle_basket_changed")
            
            data = json.loads(msg.body.decode('utf-8'))
            items = []
            for item in data["items"]:
                items.append(Item(item["itemId"], item["itemQuantity"]))
            evt = CustomerShoppingList(data["customerId"], items)
            await self.__usecase.execute(evt)

class CustomerBasketRemovedHandler:
    __usecase :StoreCustomerRemovedEventUseCase
    def __init__(self, usecase :StoreCustomerRemovedEventUseCase) -> None:
        self.__usecase = usecase

    async def handle(self, msg: IncomingMessage):
        async with msg.process():
            data = json.loads(msg.body.decode('utf-8'))
            await self.__usecase.execute(data["customerId"])