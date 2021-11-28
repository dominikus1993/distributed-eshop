from asyncio.events import AbstractEventLoop
import aio_pika
from typing import Callable, Coroutine
from ddtrace.context import Context

from ddtrace.span import Span
from common.env import get_env_or_default
from aio_pika import IncomingMessage
from aio_pika.exchange import ExchangeType
from aio_pika.robust_connection import RobustConnection
from ddtrace import tracer
from ddtrace.tracer import Tracer

class RabbitMqClient:
    __connection: RobustConnection
    __channel: aio_pika.Channel

    def __init__(self, conn: RobustConnection, ch: aio_pika.Channel):
        self.__connection = conn
        self.__channel = ch

    async def consume(self, exchange: str, queue: str, routing_key: str, consume: Callable[[IncomingMessage], Coroutine]):
        channel = self.__channel
        exch = await channel.declare_exchange(exchange, ExchangeType.TOPIC, durable=True)
        q = await channel.declare_queue(queue, durable=True, exclusive=False, auto_delete=False)
        await q.bind(exchange=exch, routing_key=routing_key)
        await q.consume(consume)

    async def close(self):
        await self.__connection.close()


class TracedRabbitMqClient(RabbitMqClient):
    def __init__(self, conn: RobustConnection, ch: aio_pika.Channel):
        RabbitMqClient.__init__(self, conn, ch)

    async def consume(self, exchange: str, queue: str, routing_key: str, consume: Callable[[IncomingMessage], Coroutine]):
        async def traced_consume(msg: IncomingMessage):
            trace_id = msg.properties.headers["x-datadog-trace-id"] if "x-datadog-trace-id" in msg.properties.headers else None
            span_id = msg.properties.headers["x-datadog-parent-id"] if "x-datadog-parent-id" in msg.properties.headers else None
            span_ctx: Context | None  = Context(int(trace_id), int(span_id)) if trace_id is not None and span_id is not None else None
            if span_ctx is not None:
                tracer.context_provider.activate(span_ctx)

            with tracer.trace("rabbitmq.consume", 'shopping-list-analytyst') as span:
                span.set_tag("exchange", msg.exchange)
                span.set_tag("topic", msg.routing_key)
                await consume(msg)
        await RabbitMqClient.consume(self, exchange, queue, routing_key, traced_consume)



async def connect(loop: AbstractEventLoop) -> RabbitMqClient:
    host = get_env_or_default('RABBITMQ_HOST', 'rabbitmq')
    port = int(get_env_or_default('RABBITMQ_PORT', '5672'))
    username = get_env_or_default('RABBITMQ_USERNAME', 'guest')
    password = get_env_or_default('RABBITMQ_PASSWORD', 'guest')
    connection = await aio_pika.connect_robust(
        host=host, port=port, login=username, password=password, loop=loop)
    channel = await connection.channel()
    return RabbitMqClient(connection, channel)

async def connect_traced(loop: AbstractEventLoop) -> RabbitMqClient:
    host = get_env_or_default('RABBITMQ_HOST', 'rabbitmq')
    port = int(get_env_or_default('RABBITMQ_PORT', '5672'))
    username = get_env_or_default('RABBITMQ_USERNAME', 'guest')
    password = get_env_or_default('RABBITMQ_PASSWORD', 'guest')
    connection = await aio_pika.connect_robust(
        host=host, port=port, login=username, password=password, loop=loop)
    channel = await connection.channel()
    return TracedRabbitMqClient(connection, channel)

