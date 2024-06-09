using System.Threading.Channels;

using Common.Types;

using EasyNetQ;
using EasyNetQ.Serialization.SystemTextJson;

using Messaging.Abstraction;
using Messaging.RabbitMq.Consumer;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

using IMessage = Messaging.Abstraction.IMessage;

namespace Messaging.Tests.Extensions;


internal sealed class TestSubscriber<T> : IMessageSubscriber<T> where T : IMessage
{
    private readonly ChannelWriter<T> _writer;
    private readonly int _take;
    private int _elements = 0;

    public TestSubscriber(ChannelWriter<T> writer, in int take)
    {
        _writer = writer;
        _take = take;
    }

    public async Task<Result<Unit>> Handle(T message, CancellationToken cancellationToken = default)
    {
        await _writer.WaitToWriteAsync(cancellationToken);
        await _writer.WriteAsync(message, cancellationToken);
        Interlocked.Increment(ref _elements);

        if (_elements == _take)
        {
            _writer.Complete();
        }

        return Result.UnitResult;
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
        services.AddSingleton(NullLoggerFactory.Instance.CreateLogger<RabbitMqMessageConsumer<T>>());
        var consumer = new RabbitMqMessageConsumer<T>(bus, services.BuildServiceProvider(),
            configuration);
        await consumer.StartAsync(cancellationToken);
        return new RabbitMqTestConsumer<T>(consumer, channel, cancellationToken);
    }
    
    public static async Task<(RabbitMqMultiMessageTestConsumer<T1>, RabbitMqMultiMessageTestConsumer<T2>)> CreateMultiMessageConsumerAsync<T1, T2>(IAdvancedBus bus, MultiMessageRabbitMqSubscriptionConfiguration configuration, int message1SubscriberTake, int message2SubscriberTake, CancellationToken cancellationToken = default)
        where T1 : IMessage
        where T2 : IMessage
    {
        var channel = Channel.CreateBounded<T1>(new BoundedChannelOptions(message1SubscriberTake) { FullMode = BoundedChannelFullMode.Wait});
        var testSubscriber = new TestSubscriber<T1>(channel, message1SubscriberTake);
        var channel2 = Channel.CreateBounded<T2>(new BoundedChannelOptions(message2SubscriberTake) { FullMode = BoundedChannelFullMode.Wait});
        var testSubscriber2 = new TestSubscriber<T2>(channel2, message2SubscriberTake);
        var services = new ServiceCollection();
        services.AddSingleton<TestSubscriber<T1>>(testSubscriber);
        services.AddSingleton<TestSubscriber<T2>>(testSubscriber2);
        services.AddSingleton<ISerializer>(new SystemTextJsonSerializer());
        services.AddSingleton(NullLoggerFactory.Instance.CreateLogger<MultiMessageRabbitMqMessageConsumer>());
        var consumer = new MultiMessageRabbitMqMessageConsumer(bus, services.BuildServiceProvider(), configuration);
        await consumer.StartAsync(cancellationToken);
        return (new RabbitMqMultiMessageTestConsumer<T1>(consumer, channel, cancellationToken), new RabbitMqMultiMessageTestConsumer<T2>(consumer, channel2, cancellationToken));
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
        await foreach (var message in _channel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
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

public sealed class RabbitMqMultiMessageTestConsumer<T>: IAsyncDisposable where T : IMessage
{

    private readonly MultiMessageRabbitMqMessageConsumer _consumer;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly Channel<T> _channel;
    
    internal RabbitMqMultiMessageTestConsumer(MultiMessageRabbitMqMessageConsumer consumer, Channel<T> channel, CancellationToken cancellationToken)
    {
        _channel = channel;
        _consumer = consumer;
        _cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

    }

    public async IAsyncEnumerable<T> Consume()
    {
        await foreach (var message in _channel.Reader.ReadAllAsync(_cancellationTokenSource.Token))
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