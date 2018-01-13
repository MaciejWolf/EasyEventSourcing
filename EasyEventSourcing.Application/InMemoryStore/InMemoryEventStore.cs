using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EasyEventSourcing.EventSourcing.Events;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.Persistence;

namespace EasyEventSourcing.Application.InMemoryStore
{
    public class InMemoryEventStore : IEventStore
    {
        private readonly IDictionary<string, List<IEvent>> store = new ConcurrentDictionary<string, List<IEvent>>();

        private readonly List<IEventObserver> eventObservers = new List<IEventObserver>();

        private int version;

        public IEnumerable<IEvent> GetEvents(EventFilter filter)
        {
            var filterDefinition = filter.GetFilter();

            IEnumerable<IEvent> events = new List<IEvent>();
            if (filterDefinition.StreamIdentifier != null)
            {
                if (store.ContainsKey(filterDefinition.StreamIdentifier.Value))
                    events = store[filterDefinition.StreamIdentifier.Value];
            }
            else
            {
                events = store.Values.SelectMany(e => e);
            }

            if (filterDefinition.Types.Any())
                events = events.Join(filterDefinition.Types, @event => @event.GetType(), type => type.Key, (@event, _) => @event);

            if (filter.Timestamp.HasValue)
                events = events.Where(e => e.Timestamp <= filter.Timestamp.Value);

            if (filterDefinition.Version != null && filterDefinition.Version.VersionMode == VersionMode.Max)
                events = events.Where(e => e.Version <= filterDefinition.Version.Version);
            else if (filterDefinition.Version != null && filterDefinition.Version.VersionMode == VersionMode.Min)
                events = events.Where(e => e.Version >= filterDefinition.Version.Version);

            events = events.OrderBy(e => e.Version);

            return events;
        }

        public void Save(List<EventStoreStream> newEvents)
        {
            foreach (var eventStoreStream in newEvents)
            {
                PersistEvents(eventStoreStream);
                DispatchEvents(eventStoreStream.Events);
            }
        }

        public Action Subscribe(IEventObserver observer)
        {
            eventObservers.Add(observer);
            return () => eventObservers.Remove(observer);
        }

        public void DeleteDatabase()
        {
            store.Clear();
        }

        private void PersistEvents(EventStoreStream eventStoreStream)
        {
            var eventList = new List<IEvent>();
            foreach (var @event in eventStoreStream.Events)
            {
                @event.Version = ++version;
                @event.Timestamp = DateTime.Now;
                eventList.Add(@event);
            }

            if (!store.ContainsKey(eventStoreStream.Id))
                store.Add(eventStoreStream.Id, eventList);
            else
                store[eventStoreStream.Id].AddRange(eventList);
        }

        private void DispatchEvents(IEnumerable<IEvent> newEvents)
        {
            foreach (var evt in newEvents)
            {
                dynamic typeAwareEvent = evt; //this cast is required to pass the correct Type to the Notify Method. Otherwise IEvent is used as the Type
                foreach (var observer in eventObservers)
                {
                    observer.Notify(typeAwareEvent);
                }
            }
        }
    }
}
