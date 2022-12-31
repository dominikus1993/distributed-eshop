using System.Threading.Channels;

using EasyNetQ;
using EasyNetQ.Serialization.SystemTextJson;

using Messaging.Abstraction;
using Messaging.RabbitMq.Consumer;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using IMessage = Messaging.Abstraction.IMessage;

namespace Messaging.Tests.Extensions;

file sealed class TestSubscriber<T> : IMessageSubscriber<T> where T : IMessage
{
    private readonly ChannelWriter<T> _writer;
    private readonly int _take;
    private int _elements = 0;

    public TestSubscriber(ChannelWriter<T> writer, in int take)
    {
        _writer = writer;
        _take = take;
    }

    public async Task Handle(T message, CancellationToken cancellationToken = default)
    {
        await _writer.WaitToWriteAsync(cancellationToken);
        await _writer.WriteAsync(message, cancellationToken);
        Interlocked.Increment(ref _elements);

        if (_elements == _take)
        {
            _writer.Complete();
        }
    }
}

public static class RabbitMqTestConsumer
{
    public static async Task<RabbitMqTestConsumer<T>> CreateAsync<T>(IAdvancedBus bus, RabbitMqSubscriptionConfiguration<T> configuration, int take = 1, CancellationToken cancellationToken = default) where T : IMessage
    {
        var channel = Channel.CreateBounded<T>(new BoundedChannelOptions(take) { FullMode = BoundedChannelFullMode.Wait});
        var testSubscriber = new TestSubscriber<T>(channel, take);
        var services = new ServiceCollection();
        services.AddSingleton<IMessageSubscriber<T>>(testSubscriber);
        services.AddSingleton<ISerializer>(new SystemTextJsonSerializer());
        services.AddSingleton(LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<RabbitMqMessageConsumer<T>>());
        var consumer = new RabbitMqMessageConsumer<T>(bus, services.BuildServiceProvider(),
            configuration);
        await consumer.StartAsync(cancellationToken);
        return new RabbitMqTestConsumer<T>(consumer, channel, cancellationToken);
    }
}

public sealed class RabbitMqTestConsumer<T>: IAsyncDisposable where T : IMessage
{

    private readonly RabbitMqMessageConsumer<T> _consumer;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Channel<T> _channel;
    
    internal RabbitMqTestConsumer(RabbitMqMessageConsumer<T> consumer, Channel<T> channel, CancellationToken cancellationToken)
    {
        _channel = channel;
        _consumer = consumer;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

    }

    public async IAsyncEnumerable<T> Consume()
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(_cancellationTokenSource.Token).WithCancellation(_cancellationTokenSource.Token))
        {
            yield return message;
        }
    }
    
    public async Task<T?> ConsumeOne()
    {
        await _channel.Reader.WaitToReadAsync(_cancellationTokenSource.Token);
        return await _channel.Reader.ReadAsync(_cancellationTokenSource.Token);
    }


    public async ValueTask DisposeAsync()
    {
        await _consumer.StopAsync(_cancellationTokenSource.Token);
        _cancellationTokenSource.Dispose();
        _channel.Writer.TryComplete();
        _consumer.Dispose();
    }
}