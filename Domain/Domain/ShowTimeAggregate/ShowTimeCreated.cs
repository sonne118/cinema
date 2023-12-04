using ApiApplication.Domain.Common.Models;

namespace ApiApplication.Domain.ShowTimeAggregate
{

    public record ShowTimeCreated(ShowTime Menu) : IDomainEvent;

}