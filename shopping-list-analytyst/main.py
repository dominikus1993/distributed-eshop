from typing import Optional
import asyncio
from fastapi import FastAPI
from ddtrace import patch, config, tracer
from aio_pika import IncomingMessage
import pymongo
from common.env import get_env_or_default
from core.usecase import StoreCustomerShoppingListHistoryUseCase
from handlers.worker import CustomerBasketChangedHandler
from infrastructure.data import MongoCustomerShoppingListHistoryWriter
from infrastructure.rabbitmq import RabbitMqClient, connect

config.fastapi['service_name'] = 'shopping-list-analytyst'

patch(fastapi=True)
app = FastAPI()

client: RabbitMqClient | None = None
mongo = pymongo.MongoClient(get_env_or_default("MONGO_CONNECTION", "mongodb://db:27017/"))
usecase = StoreCustomerShoppingListHistoryUseCase(MongoCustomerShoppingListHistoryWriter(mongo))
handler = CustomerBasketChangedHandler(usecase)

@app.get("/")
async def read_root():
    return {"Hello": "World"}


@app.get("/items/{item_id}")
async def read_item(item_id: int, q: Optional[str] = None):
    return {"item_id": item_id, "q": q}

@app.get("/items")
async def read_items(q: Optional[str] = None):
    for x in range(10):
        yield { "item_id": x}


@tracer.wrap("rabbitmq_consume", 'shopping-list-analytyst')
async def print_msg(msg: IncomingMessage):
    async with msg.process():
        print("[x] %r" % msg.body)


async def init_consumer(loop):
    client = await connect(loop=loop)
    # use the same loop to consume
    await client.consume("basket", "test", "changed", handler.handle)       
    

@app.on_event("shutdown")
async def shutdown_event():
    if client is not None:
        print("Close RabbitMq connection")
        await client.close()



@app.on_event('startup')
async def startup():
    loop = asyncio.get_event_loop()
    asyncio.ensure_future(init_consumer(loop))
