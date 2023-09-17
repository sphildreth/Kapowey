using Kapowey.Core.Common.Models;
using MediatR;
using MediatR.Pipeline;
using Microsoft.Extensions.Logging;

namespace Kapowey.Core.Common.ExceptionHandlers;
public class ServerExceptionHandler<TRequest, TResponse, TException> : IRequestExceptionHandler<TRequest, TResponse, TException>
    where TRequest : IRequest<Result<int>>
    where TResponse: Result<int>
    where TException : ServerException
{
    private readonly ILogger<ServerExceptionHandler<TRequest, TResponse, TException>> _logger;

    public ServerExceptionHandler(ILogger<ServerExceptionHandler<TRequest, TResponse, TException>> logger)
    {
        _logger = logger;
    }

    public Task Handle(TRequest request, TException exception, RequestExceptionHandlerState<TResponse> state, CancellationToken cancellationToken)
    {
        state.SetHandled((TResponse)Result<int>.Failure(new string[] { exception.Message }));
        return Task.CompletedTask;
    }


}
