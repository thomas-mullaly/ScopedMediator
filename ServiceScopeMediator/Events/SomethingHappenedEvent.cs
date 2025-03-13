using MediatR;

namespace ServiceScopeMediator.Events;

public record SomethingHappenedEvent(int StuffId) : INotification;