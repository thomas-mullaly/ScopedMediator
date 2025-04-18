﻿using MediatR;
using Microsoft.EntityFrameworkCore;
using ServiceScopeMediator.Sample.Data;
using ServiceScopeMediator.Sample.Events;

namespace ServiceScopeMediator.Sample.Handlers;

public class SomethingHappenedEventHandler : INotificationHandler<SomethingHappenedEvent>
{
    private readonly AppDbContext _context;

    public SomethingHappenedEventHandler(AppDbContext context)
    {
        _context = context;
    }

    public async Task Handle(SomethingHappenedEvent notification, CancellationToken cancellationToken)
    {
        var stuff = await _context.Stuffs.SingleAsync(s => s.Id == notification.StuffId, cancellationToken);
    }
}