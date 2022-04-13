from asyncio.events import AbstractEventLoop
import aio_pika
from typing import Callable, Coroutine
from opentelemetry.sdk.trace import TracerProvider
from ddtrace.span import Span
from common.env import get_env_or_default
from aio_pika import IncomingMessage
from aio_pika.exchange import ExchangeType
from aio_pika.robust_connection import RobustConnection
from opentelemetry.propagators.textmap import TextMapPropagator
from opentelemetry.propagators.textmap import Getter
from typing import Any, Callable, List, Optional
from opentelemetry import context, propagate, trace
from opentelemetry.trace import SpanKind, Tracer

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

class _PikaGetter(Getter):  # type: ignore
    def get(self, carrier: Any, key: str) -> Optional[List[str]]:
        value = carrier.get(key, None)
        print(f'_PikaGetter.get: {key}={value}')
        if value is None:
            return None
        return [value]

    def keys(self, carrier: Any) -> List[str]:
        return []


_pika_getter = _PikaGetter()

class TracedRabbitMqClient(RabbitMqClient):
    __trace_provider: TracerProvider

    def __init__(self, conn: RobustConnection, ch: aio_pika.Channel, tp: TracerProvider):
        self.__trace_provider = tp
        RabbitMqClient.__init__(self, conn, ch)

    async def consume(self, exchange: str, queue: str, routing_key: str, consume: Callable[[IncomingMessage], Coroutine]):
        async def traced_consume(msg: IncomingMessage):
            ctx = propagate.extract(msg.properties.headers, getter=_pika_getter)
            if not ctx:
                ctx = context.get_current()
            token = context.attach(ctx)
            with self.__trace_provider.get_tracer(__name__).start_as_current_span(name="consume",kind=SpanKind.CONSUMER) as span:
                span.set_attribute("messaging.destination", msg.exchange)
                span.set_attribute("messaging.system", "rabbitmq")
                span.set_attribute("messaging.routing_key", msg.routing_key)
                span.set_attribute("messaging.protocol", "amqp")
                span.set_attribute("messaging.protocol_version", "0-9-1")
                await consume(msg)
            context.detach(token)
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

async def connect_traced(loop: AbstractEventLoop, tp: TracerProvider) -> RabbitMqClient:
    host = get_env_or_default('RABBITMQ_HOST', 'rabbitmq')
    port = int(get_env_or_default('RABBITMQ_PORT', '5672'))
    username = get_env_or_default('RABBITMQ_USERNAME', 'guest')
    password = get_env_or_default('RABBITMQ_PASSWORD', 'guest')
    connection = await aio_pika.connect_robust(
        host=host, port=port, login=username, password=password, loop=loop)
    channel = await connection.channel()
    return TracedRabbitMqClient(connection, channel, tp)

