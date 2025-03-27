using MediatR;

namespace ServiceScopeMediator.Sample.Events;

public record SomethingHappenedEvent(int StuffId) : INotification;