using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.EventSourcing.Handlers
{
    public interface IEventObserver
    {
        void Notify<TEvent>(TEvent evt) where TEvent : IEvent;
    }
}