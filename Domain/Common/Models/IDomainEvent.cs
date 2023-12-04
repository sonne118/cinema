using MediatR;

namespace ApiApplication.Domain.Common.Models
{

    public interface IDomainEvent : INotification
    {
    }

}