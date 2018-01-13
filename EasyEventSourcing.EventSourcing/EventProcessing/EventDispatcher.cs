using EasyEventSourcing.EventSourcing.Events;
using EasyEventSourcing.EventSourcing.Handlers;

namespace EasyEventSourcing.EventSourcing.EventProcessing
{
    public class EventDispatcher : IEventDispatcher
    {
        private readonly IEventHandlerFactory factory;

        public EventDispatcher(IEventHandlerFactory factory)
        {
            this.factory = factory;
        }

        public void Send<TEvent>(TEvent evt) where TEvent : IEvent
        {
            var handlers = this.factory.ResolveEventHandler(evt);
            foreach (var eventHandler in handlers)
            {
                eventHandler.Handle(evt);
            }
        }
    }
}