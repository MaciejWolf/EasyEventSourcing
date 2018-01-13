using System;
using System.Collections.Generic;
using EasyEventSourcing.EventSourcing.Domain;

namespace EasyEventSourcing.EventSourcing.Persistence
{
    public class Repository : IRepository
    {
        private readonly IEventStore eventStore;
        public Repository(IEventStore eventStore)
        {
            this.eventStore = eventStore;
        }

        public T GetById<T>(Guid id) where T : EventStream, new()
        {
            // TODO GetMongoDb().Load("Snapshot").Where(StreamId == id)
            var streamItem = new T(); // TODO Cache ME / And or SnapShot ME!
            streamItem.id = id;
            var history = this.eventStore.GetEvents(new EventFilter { StreamIdentifier = streamItem.StreamIdentifier });
            streamItem.LoadFromHistory(history);
            return streamItem;
        }

        public void Save(params EventStream[] streamItems)
        {
            var newEvents = new List<EventStoreStream>();
            foreach (var item in streamItems)
            {
                newEvents.Add(new EventStoreStream(item.StreamIdentifier, item.GetUncommitedChanges()));
            }

            this.eventStore.Save(newEvents);

            foreach (var item in streamItems)
            {
                item.Commit();
            }
        }
    }
}