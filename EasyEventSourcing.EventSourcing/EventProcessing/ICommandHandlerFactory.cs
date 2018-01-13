using EasyEventSourcing.EventSourcing.Handlers;

namespace EasyEventSourcing.EventSourcing.EventProcessing
{
    public interface ICommandHandlerFactory
    {
        void RegisterCommandHandler<TCommandHandler>() where TCommandHandler : IHandler;
    }
}