using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.EventSourcing.Handlers
{
    public interface IEventDispatcher
    {
        void Send<TEvent>(TEvent evt) where TEvent : IEvent;
    }
}