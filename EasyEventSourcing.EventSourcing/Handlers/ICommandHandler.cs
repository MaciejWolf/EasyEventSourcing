using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.EventSourcing.Handlers
{
    public interface ICommandHandler<in TCommand>: IHandler where TCommand : ICommand
    {
        void Handle(TCommand cmd);
    }
}