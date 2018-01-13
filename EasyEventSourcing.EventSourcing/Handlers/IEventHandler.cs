using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.EventSourcing.Handlers
{
    public interface IEventHandler<in TEvent>: IHandler where TEvent : IEvent
    {
        void Handle(TEvent @event);
    }
}