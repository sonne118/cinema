using ApiApplication.Domain.ShowTimeAggregate;
using Application.Common.Interfaces.Persistence;
using ErrorOr;

using MediatR;

namespace BuberDinner.Application.Menus.Queries.ListMenus;

public class ListShowTimeQueryHandler : IRequestHandler<ListShowTimeQuery, ErrorOr<List<ShowTime>>>
{
    private readonly IShowTimeRepository _menuRepository;

    public ListShowTimeQueryHandler(IShowTimeRepository menuRepository)
    {
        _menuRepository = menuRepository;
    }

    public async Task<ErrorOr<List<ShowTime>>> Handle(ListShowTimeQuery query, CancellationToken cancellationToken)
    {
        //var hostId = HostId.Create(query.HostId);

        //return await _menuRepository.ListAsync(hostId);
        return null;
    }
}