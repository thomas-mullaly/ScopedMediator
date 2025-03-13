using MediatR;
using ServiceScopeMediator.Data;

namespace ServiceScopeMediator.PipelineBehaviors;

public class TransactionBehavior<TRequest,TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    private readonly AppDbContext _dbContext;
    private readonly ILogger<TransactionBehavior<TRequest, TResponse>> _logger;

    public TransactionBehavior(AppDbContext dbContext, ILogger<TransactionBehavior<TRequest, TResponse>> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Starting transaction...");
        await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

        var response = await next();

        _logger.LogInformation("Committing transaction...");
        await transaction.CommitAsync(cancellationToken);

        return response;
    }
}