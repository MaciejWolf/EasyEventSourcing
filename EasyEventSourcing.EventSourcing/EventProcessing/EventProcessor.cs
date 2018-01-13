using System;
using EasyEventSourcing.EventSourcing.Events;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.Persistence;

namespace EasyEventSourcing.EventSourcing.EventProcessing
{
    public class EventProcessor : IEventObserver
    {
        private readonly IEventDispatcher dispatcher;
        private Action unsubscribe;

        public EventProcessor(IEventStore store, IEventDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.unsubscribe = store.Subscribe(this);
        }

        public void Notify<TEvent>(TEvent evt) where TEvent : IEvent
        {
            dispatcher.Send(evt);
        }
    }
}
