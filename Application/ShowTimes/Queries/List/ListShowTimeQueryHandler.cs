using ApiApplication.Application.ShowTimes.Query.List;
using ApiApplication.Common.Interfaces.Persistence;
using ApiApplication.Domain.Domain.ShowTimeAggregate;
using ErrorOr;
using MediatR;

//namespace

//public class ListShowTimeQueryHandler : IRequestHandler<ListShowTimeQuery, ErrorOr<List<ShowTime>>>
//{
//    private readonly IShowTimeRepository _menuRepository;

//    public ListShowTimeQueryHandler(IShowTimeRepository menuRepository)
//    {
//        _menuRepository = menuRepository;
//    }

//    //public async Task<ErrorOr<List<ShowTime>>> Handle(ListShowTimeQuery query, CancellationToken cancellationToken)
//    public  Task Handle(ListShowTimeQuery query, CancellationToken cancellationToken)

//    {
//        //var hostId = HostId.Create(query.HostId);

//        //return await _menuRepository.ListAsync(hostId);
//        return   Task.CompletedTask;
//    }
//}