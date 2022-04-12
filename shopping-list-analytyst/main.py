from typing import Optional
import asyncio
from fastapi import FastAPI
from aio_pika import IncomingMessage
import pymongo
from opentelemetry.instrumentation.fastapi import FastAPIInstrumentor
from common.env import get_env_or_default
from core.usecase import ReadCustomerShoppingListHistoryUseCase, StoreCustomerChangedEventUseCase, StoreCustomerRemovedEventUseCase
from handlers.worker import CustomerBasketChangedHandler, CustomerBasketRemovedHandler
from infrastructure.data import MongoCustomerShoppingListHistoryReader, MongoCustomerShoppingListHistoryWriter
from infrastructure.rabbitmq import RabbitMqClient, connect_traced
from opentelemetry.instrumentation.pymongo import PymongoInstrumentor

app = FastAPI()

client: RabbitMqClient | None = None
PymongoInstrumentor().instrument()
mongo = pymongo.MongoClient(get_env_or_default("MONGO_CONNECTION", "mongodb://db:27017/"))
writer = MongoCustomerShoppingListHistoryWriter(mongo)
store_customer_changed_event_usecase = StoreCustomerChangedEventUseCase(writer)
customer_basket_changed_handler = CustomerBasketChangedHandler(store_customer_changed_event_usecase)
store_customer_removed_event_usecase = StoreCustomerRemovedEventUseCase(writer)
customer_basket_removed_handler = CustomerBasketRemovedHandler(store_customer_removed_event_usecase)
reader = MongoCustomerShoppingListHistoryReader(mongo)
read_customer_shopping_list_history_usecase = ReadCustomerShoppingListHistoryUseCase(reader)

@app.get("/")
async def read_root():
    return {"Hello": "World"}


@app.get("/logs/{customer_id}")
async def read_item(customer_id: int, q: Optional[str] = None):
    result = [log async for log in read_customer_shopping_list_history_usecase.execute(customer_id)]
    return result

@app.get("/items")
async def read_items(q: Optional[str] = None):
    for x in range(10):
        yield { "item_id": x}


async def handle_basket_removed(msg: IncomingMessage):
    await customer_basket_removed_handler.handle(msg)

async def handle_basket_changed(msg: IncomingMessage):
    await customer_basket_changed_handler.handle(msg)

async def init_consumer(loop):
    client = await connect_traced(loop=loop)
    # use the same loop to consume
    await asyncio.gather(
         client.consume("basket", "analytyst_basket_changed", "changed", handle_basket_changed),
         client.consume("basket", "analytyst_basket_removed", "removed", handle_basket_removed),
    )    
    

@app.on_event("shutdown")
async def shutdown_event():
    if client is not None:
        print("Close RabbitMq connection")
        await client.close()
    mongo.close()



@app.on_event('startup')
async def startup():
    FastAPIInstrumentor.instrument_app(app)
    loop = asyncio.get_event_loop()
    asyncio.ensure_future(init_consumer(loop))
