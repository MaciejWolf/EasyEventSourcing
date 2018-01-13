using System;
using System.Collections.Generic;
using EasyEventSourcing.EventSourcing.Events;
using EasyEventSourcing.EventSourcing.Handlers;

namespace EasyEventSourcing.EventSourcing.Persistence
{
    public interface IEventStore
    {
        IEnumerable<IEvent> GetEvents(EventFilter filter);

        void Save(List<EventStoreStream> newEvents);

        Action Subscribe(IEventObserver observer);

        void DeleteDatabase();
    }
}