using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceScopeMediator.Sample.Data;
using ServiceScopeMediator.Sample.Dtos;
using ServiceScopeMediator.Sample.Events;
using ServiceScopeMediator.Sample.Model;
using ServiceScopeMediator.Sample.Requests;

namespace ServiceScopeMediator.Sample.Handlers;

public class GetStuffRequestHandler : IRequestHandler<GetStuffRequest, List<StuffDto>>
{
    private readonly AppDbContext _dbContext;
    private readonly IMediator _mediator;

    public GetStuffRequestHandler(AppDbContext dbContext, IMediator mediator)
    {
        _dbContext = dbContext;
        _mediator = mediator;
    }

    public async Task<List<StuffDto>> Handle(GetStuffRequest request, CancellationToken cancellationToken)
    {
        var newStuff = new Stuff
        {
            Name = Guid.NewGuid().ToString()
        };

        await _dbContext.Stuffs.AddAsync(newStuff, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var stuffs = await _dbContext.Stuffs.ToListAsync(cancellationToken);

        await _mediator.Publish(new SomethingHappenedEvent(newStuff.Id), cancellationToken);
        return stuffs.Select(s => new StuffDto { Name = s.Name }).ToList();
    }
}