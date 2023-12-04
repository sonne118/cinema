using ApiApplication.Domain.ShowTimeAggregate;
using ErrorOr;
using MediatR;

namespace BuberDinner.Application.Menus.Queries.ListMenus;

public record ListShowTimeQuery(string HostId)
    : IRequest<ErrorOr<List<ShowTime>>>;