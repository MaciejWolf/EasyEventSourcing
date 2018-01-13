using System;
using EasyEventSourcing.Data.EventStore;
using EasyEventSourcing.EventSourcing.Domain;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace EasyEventSourcing.Data.AggregateSnapshot
{
    public class MongoDbSnapshotManager
    {
        private static readonly string EventCollection = "snapshot";

        private static MongoDbEventStore instance;

        private static readonly string DbName = "event-sourcing";

        public static MongoDbEventStore Instance
        {
            get
            {
                // TODO fix snapshot manager
                //instance = instance ?? new MongoDbEventStore();
                return instance;
            }
        }

        public object GetById(StreamIdentifier streamId)
        {
            return null;
        }

        public void Save(Aggregate aggregate)
        {
            var snapshot = new SnapshotPersistence {
                AggregateId = aggregate.StreamIdentifier.Value,
                AggregateClass = aggregate.GetType().FullName,
                 Data = JsonConvert.SerializeObject(aggregate),
                 Id = new ObjectId(DateTime.Now, 1, 1, aggregate.Version )

            };

            var collection = GetMongoDbCollection(EventCollection);
            collection.InsertOne(snapshot);
        }

        public void DeleteDatabase()
        {
            var collection = GetMongoDbCollection(EventCollection);
            collection.DeleteMany(@event => true);
        }

        private static IMongoCollection<SnapshotPersistence> GetMongoDbCollection(string collectionName)
        {
            //var mongoClient = new MongoClient("mongodb://root:DkYQtqwqW7ia@51.140.85.21");
            var mongoClient = new MongoClient("mongodb://localhost:27017");
            var database = mongoClient.GetDatabase(DbName, new MongoDatabaseSettings());
            var collection = database.GetCollection<SnapshotPersistence>(collectionName);
            return collection;
        }
    }
}
