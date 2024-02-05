using ApiApplication.Domain.Domain.ShowTimeAggregate;
using ErrorOr;
using MediatR;

namespace ApiApplication.Application.ShowTimes.Query.List;

public record ListShowTimeQuery(string HostId)
    : IRequest<ErrorOr<List<ShowTime>>>;