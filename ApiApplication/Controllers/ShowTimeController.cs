using ApiApplication.Contracts.Menus;
using BuberDinner.Application.Menus.Commands.CreateMenu;
using BuberDinner.Application.Menus.Queries.ListMenus;
using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace BuberDinner.Api.Controllers;

[Route("hosts/{hostId}/menus")]
public class ShowTimeController : ApiController
{
    private readonly IMapper _mapper;
    private readonly ISender _mediator;

    public ShowTimeController(IMapper mapper, ISender mediator)
    {
        _mapper = mapper;
        _mediator = mediator;
    }

    [HttpPost]
    public async Task<IActionResult> CreateMenu(
        CreateMenuRequest request,
        string hostId)
    {
        var command = _mapper.Map<CreateMenuCommand>((request, hostId));

        var createMenuResult = await _mediator.Send(command);

        return createMenuResult.Match(
            menu => Ok(_mapper.Map<MenuResponse>(menu)),
            errors => Problem(errors));
    }

    [HttpGet]
    public async Task<IActionResult> ListMenus(string hostId)
    {
        var query = _mapper.Map<ListShowTimeQuery>(hostId);

        var listMenusResult = await _mediator.Send(query);

        return listMenusResult.Match(
            menus => Ok(menus.Select(menu => _mapper.Map<MenuResponse>(menu))),
            errors => Problem(errors));
    }
}