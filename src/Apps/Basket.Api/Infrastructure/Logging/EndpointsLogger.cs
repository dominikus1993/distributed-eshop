using FluentValidation.Results;

namespace Basket.Api.Infrastructure.Logging;

internal static partial class EndpointsLogger
{
    [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Warning,
            Message = "Invalid Request, ValidationFailures: '{ValidationFailures}'")]
        public static partial void LogInvalidRequest(
            this ILogger logger, List<ValidationFailure> validationFailures);
    
    
}
