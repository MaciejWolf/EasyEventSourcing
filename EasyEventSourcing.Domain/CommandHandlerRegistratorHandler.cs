using EasyEventSourcing.Domain.Orders;
using EasyEventSourcing.Domain.Products;
using EasyEventSourcing.Domain.Shipping;
using EasyEventSourcing.Domain.Store;
using EasyEventSourcing.EventSourcing.EventProcessing;

namespace EasyEventSourcing.Domain
{
    public class CommandHandlerRegistratorHandler : ICommandHandlerRegistrator
    {
        public void Register(ICommandHandlerFactory registration)
        {
            registration.RegisterCommandHandler<OrderHandler>();
            registration.RegisterCommandHandler<ProductHandler>();
            registration.RegisterCommandHandler<ShoppingCartHandler>();
        }

        // TODO: Allow to register SAGA's
        public void RegisterSaga(ICommandHandlerFactory registration)
        {
            registration.RegisterCommandHandler<OrderEventHandler>();
        }
    }
}
