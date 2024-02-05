using ApiApplication.Common.Interfaces.Persistence;
using ApiApplication.Domain.Domain.ShowTimeAggregate;
using ErrorOr;
using MediatR;

namespace  ApiApplication.Application.ShowTimes.Command.CreateShowTime
{

    public class CreateShowTimeCommandHandler : IRequestHandler<CreateShowTimeCommand, ErrorOr<ShowTime>>
    {
        private readonly IShowTimeRepository _showTimeRepository;

        public CreateShowTimeCommandHandler(IShowTimeRepository menuRepository)
        {
            _showTimeRepository = menuRepository;
        }

        public async Task<ErrorOr<ShowTime>> Handle(CreateShowTimeCommand command, CancellationToken cancellationToken)
        {
            var menu = ShowTime.Create(
                movieId: MovieId.Create( Guid.Parse(command.MovieId)),
                sessionDate: DateTime.Parse(command.sessionDate),
                auditoriumId: AuditoriumId.Create(Guid.Parse(command.auditoriumId)));
            //    sections: command.Tickets.ConvertAll(section => MenuSection.Create(
            //    section.Name,
            //    section.Description,
            //    section.Items.ConvertAll(item => MenuItem.Create(
            //        item.Name,
            //        item.Description))))
            //        );

            await _showTimeRepository.AddAsync(menu);

            return menu;
        }
    }
}