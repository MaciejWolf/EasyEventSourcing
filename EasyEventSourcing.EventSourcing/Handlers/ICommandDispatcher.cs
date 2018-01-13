using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.EventSourcing.Handlers
{
    public interface ICommandDispatcher
    {
        void Send<TCommand>(TCommand command) where TCommand : ICommand;
    }
}