using System;
using MongoDB.Bson;

namespace EasyEventSourcing.Data.EventStore
{
    internal class EventPersistance
    {
        public ObjectId Id { get; set; }

        public int Version { get; set; }

        public DateTime Timestamp { get; set; }

        public string AggregateId { get; set; }

        public string EventClass { get; set; }

        public string Data { get; set; }
    }
}
