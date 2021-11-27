from asyncio.events import AbstractEventLoop
import aio_pika
from typing import Callable, Coroutine
from common.env import get_env_or_default
from aio_pika.exchange import ExchangeType
from aio_pika.robust_connection import RobustConnection


class RabbitMqClient:
    __connection: RobustConnection
    __channel: aio_pika.Channel

    def __init__(self, conn: RobustConnection, ch: aio_pika.Channel):
        self.__connection = conn
        self.__channel = ch

    async def consume(self, exchange: str, queue: str, routing_key: str, consume: Callable[[str], Coroutine]):
        channel = self.__channel
        exch = await channel.declare_exchange(exchange, ExchangeType.TOPIC, durable=True)
        q = await channel.declare_queue(queue, durable=True, exclusive=False, auto_delete=False)
        await q.bind(exchange=exch, routing_key=routing_key)
        async with q.iterator() as queue_iter:
            async for message in queue_iter:
                async with message.process():
                    await consume(message.body)

    async def close(self):
        await self.__connection.close()


async def connect(loop: AbstractEventLoop) -> RabbitMqClient:
    host = get_env_or_default('RABBITMQ_HOST', 'rabbitmq')
    port = int(get_env_or_default('RABBITMQ_PORT', '5672'))
    username = get_env_or_default('RABBITMQ_USERNAME', 'guest')
    password = get_env_or_default('RABBITMQ_PASSWORD', 'guest')
    connection = await aio_pika.connect_robust(
        host=host, port=port, login=username, password=password, loop=loop)
    channel = await connection.channel()
    return RabbitMqClient(connection, channel)
