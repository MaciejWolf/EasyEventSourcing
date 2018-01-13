using System;
using System.Collections.Generic;
using System.Linq;
using EasyEventSourcing.EventSourcing.EventProcessing;
using EasyEventSourcing.EventSourcing.Events;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.Persistence;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace EasyEventSourcing.Data.EventStore
{
    public class MongoDbEventStore : IEventStore
    {
        private readonly ITypeResolver typeResolver;
        private readonly List<IEventObserver> eventObservers = new List<IEventObserver>();

        private static readonly string EventCollection = "events";

        private int version;

        private static readonly string DbName = "event-sourcing";

        public MongoDbEventStore(ITypeResolver typeResolver)
        {
            this.typeResolver = typeResolver;
            version = GetVersion();
        }

        public IEnumerable<IEvent> GetEvents(EventFilter filter)
        {
            var collection = GetMongoDbCollection(EventCollection);

            var builder = Builders<EventPersistance>.Filter;
            FilterDefinition<EventPersistance> mongofilter = null;

            var filterDefinition = filter.GetFilter();

            foreach (var type in filterDefinition.Types)
            {
                if (mongofilter == null)
                {
                    mongofilter = builder.Eq("EventClass", type.Key.AssemblyQualifiedName);
                    //if (type.Value != null)
                    //    mongofilter = builder.And(mongofilter, builder.Eq("AggregateId", type.Value.Value));
                }
                else
                {
                    var innerFilter = builder.Eq("EventClass", type.Key.AssemblyQualifiedName);
                    //if (type.Value != null)
                    //    innerFilter = builder.And(innerFilter, builder.Eq("AggregateId", type.Value.Value));

                    mongofilter = mongofilter | innerFilter;
                }
            }

            if (filterDefinition.StreamIdentifier != null)
                if (mongofilter == null)
                    mongofilter = builder.Eq("AggregateId", filterDefinition.StreamIdentifier.Value);
                else
                    mongofilter = builder.And(mongofilter, builder.Eq("AggregateId", filterDefinition.StreamIdentifier.Value));

            if (filterDefinition.Version != null)
            {
                if (filterDefinition.Version.VersionMode == VersionMode.Max)
                {
                    if (mongofilter == null)
                        mongofilter = builder.Lte("version", filterDefinition.Version.Version);
                    else
                        mongofilter = builder.And(mongofilter, builder.Lte("version", filterDefinition.Version.Version));
                }
                else if (filterDefinition.Version.VersionMode == VersionMode.Min)
                {
                    if (mongofilter == null)
                        mongofilter = builder.Gte("version", filterDefinition.Version.Version);
                    else
                        mongofilter = builder.And(mongofilter, builder.Gte("version", filterDefinition.Version.Version));
                }
            }

            if (filterDefinition.Timestamp.HasValue)
                if (mongofilter == null)
                    mongofilter = builder.Eq("Timestamp", filterDefinition.Timestamp.Value);
                else
                    mongofilter = builder.And(mongofilter, builder.Lte("Timestamp", filterDefinition.Timestamp.Value));

            var allRawEvents = collection
                .Find(mongofilter)
                .ToList();

            var allEvents = allRawEvents
                .Select((@event, i) =>
                {
                    var evt = (Event)JsonConvert.DeserializeObject(@event.Data, typeResolver.Lookup(@event.EventClass));
                    evt.Timestamp = @event.Timestamp;
                    evt.Version = @event.Version;
                    return evt;
                })
                .ToList();

            return allEvents;
        }

        //private Object thisLock = new Object();
        public void Save(List<EventStoreStream> newEvents)
        {
            //lock (thisLock)
            //{
            //    version = GetVersion();
            foreach (var eventStoreStream in newEvents)
            {
                PersistEvents(eventStoreStream); // Save, if no error
                DispatchEvents(eventStoreStream.Events); // Dispatch
            }
            //}
        }

        private void PersistEvents(EventStoreStream eventStoreStream)
        {
            var collection = GetMongoDbCollection(EventCollection);

            var events = eventStoreStream.Events
                 .Select(@event =>
                    {
                        var timestamp = DateTime.Now;
                        @event.Timestamp = timestamp;
                        var id = new ObjectId(timestamp, 1, 1, ++version);
                        return new EventPersistance
                        {
                            Id = id,
                            Version = version, // id.Increment,
                            Timestamp = timestamp,
                            AggregateId = eventStoreStream.Id.ToString(),
                            EventClass = @event.GetType().AssemblyQualifiedName,
                            Data = JsonConvert.SerializeObject(@event)
                        };
                    })
                 .ToList();

            collection.InsertMany(events);

            // TODO handle potential failures
        }

        private int GetVersion()
        {
            var collection = GetMongoDbCollection(EventCollection);

            if (collection.AsQueryable().Any())
                return collection.AsQueryable()
                    .Max(@event => @event.Version);

            return 0;
        }

        private void DispatchEvents(IEnumerable<IEvent> newEvents)
        {
            foreach (var evt in newEvents)
            {
                NotifySubscribers(evt);
            }
        }

        private void NotifySubscribers(IEvent evt)
        {
            dynamic typeAwareEvent = evt; //this cast is required to pass the correct Type to the Notify Method. Otherwise IEvent is used as the Type
            foreach (var observer in eventObservers)
            {
                observer.Notify(typeAwareEvent);
            }
        }

        public Action Subscribe(IEventObserver observer)
        {
            this.eventObservers.Add(observer);
            return () => this.Unsubscribe(observer);
        }

        public void DeleteDatabase()
        {
            var collection = GetMongoDbCollection(EventCollection);
            collection.DeleteMany(@event => true);

            version = -1;
        }

        private void Unsubscribe(IEventObserver observer)
        {
            eventObservers.Remove(observer);
        }

        private static IMongoCollection<EventPersistance> GetMongoDbCollection(string collectionName)
        {
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase(DbName, new MongoDatabaseSettings());
            var collection = database.GetCollection<EventPersistance>(collectionName);
            return collection;
        }
    }
}
