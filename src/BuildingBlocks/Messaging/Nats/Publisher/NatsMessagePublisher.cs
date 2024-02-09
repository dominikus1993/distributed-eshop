using System.Diagnostics.CodeAnalysis;

using Common.Types;

using Messaging.Abstraction;
using Messaging.Nats.Configuration;

using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;

namespace Messaging.Nats.Publisher;

public sealed class NatsMessagePublisher<T> : IMessagePublisher<T> where T : IMessage
{
    private readonly NatsConnection _connection;
    private readonly NatsPublisherConfig<T> _publisherConfig;
    public NatsMessagePublisher(NatsConnection connection, NatsPublisherConfig<T> publisherConfig)
    {
        _connection = connection;
        _publisherConfig = publisherConfig;
    }

    public async Task<Result<Unit>> Publish([NotNull] T message, IMessageContext? ctx = null, CancellationToken cancellationToken = default)
    {
        try
        {
            var header = new NatsHeaders()
            {
                
            }
            await _connection.PublishAsync(_publisherConfig.Subject, message, header, cancellationToken: cancellationToken);
            
            return Result.UnitResult;
        }
        catch (Exception e)
        {
            return Result.Failure<Unit>(e);
        }
    }
}