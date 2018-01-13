using EasyEventSourcing.Domain.Read.Basket;
using EasyEventSourcing.Domain.Read.Products;
using EasyEventSourcing.EventSourcing.EventProcessing;

namespace EasyEventSourcing.Domain.Read
{
    public class EventHandlerRegistratorHandler : IEventHandlerRegistrator
    {
        public void Register(IEventHandlerFactory registration)
        {
            registration.RegisterHandler<BasketEventHandler, BasketsReadModel>();
            registration.RegisterHandler<ProductEventHandler, ProductsReadModel>();
        }
    }
}
