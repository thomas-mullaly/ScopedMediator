using MediatR;
using ServiceScopeMediator.Dtos;

namespace ServiceScopeMediator.Requests;

public class GetStuffRequest : IRequest<List<StuffDto>>;