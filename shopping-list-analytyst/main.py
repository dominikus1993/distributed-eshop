from typing import Optional
import asyncio
from fastapi import FastAPI
from ddtrace import patch, config

from infrastructure.rabbitmq import connect

config.fastapi['service_name'] = 'shopping-list-analytyst'

patch(fastapi=True)
app = FastAPI()


@app.get("/")
async def read_root():
    return {"Hello": "World"}


@app.get("/items/{item_id}")
async def read_item(item_id: int, q: Optional[str] = None):
    return {"item_id": item_id, "q": q}


async def print_msg(msg: str):
    print(msg)

@app.on_event('startup')
async def startup():
    loop = asyncio.get_event_loop()
    client = await connect(loop=loop)
    # use the same loop to consume
    asyncio.ensure_future(client.consume("basket", "test", "changed", print_msg))
