using MongoDB.Bson;

namespace EasyEventSourcing.Data.AggregateSnapshot
{
    internal class SnapshotPersistence
    {
        public ObjectId Id { get; set; }

        public int Version => Id.Increment;

        public string AggregateId { get; set; }

        public string AggregateClass { get; set; }

        public string Data { get; set; }
    }
}
