using FluentValidation;
using Kapowey.Core.Common.Models;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Kapowey.Core.Common.ExceptionHandlers;
public class ValidationExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result<int>>
    where TResponse: Result<int>
    where TException : ValidationException
{
    private readonly ILogger<ValidationExceptionHandler<TRequest, TResponse, TException>> _logger;

    public ValidationExceptionHandler(ILogger<ValidationExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        state.SetHandled((TResponse)Result<int>.Failure(exception.Errors.Select(x => x.ErrorMessage).Distinct().ToArray()));
        return Task.CompletedTask;
    }
}
