using MediatR;
using ServiceScopeMediator.Sample.Dtos;

namespace ServiceScopeMediator.Sample.Requests;

public class GetStuffRequest : IRequest<List<StuffDto>>;