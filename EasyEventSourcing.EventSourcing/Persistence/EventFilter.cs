using EasyEventSourcing.EventSourcing.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.EventSourcing.Persistence
{
    public class EventFilter
    {
        public DateTime? Timestamp { get; set; }

        public EventVersion Version { get; set; }

        public StreamIdentifier StreamIdentifier { get; set; }

        public ICollection<KeyValuePair<Type, StreamIdentifier>> Types { get; private set; } = new List<KeyValuePair<Type, StreamIdentifier>>();

        public void AddTypeFilter<TEvent>() where TEvent : IEvent
        {
            Types.Add(new KeyValuePair<Type, StreamIdentifier>(typeof(TEvent), null));
        }

        public void AddTypeFilter(Type[] types)
        {
            Types = types.Select(type => new KeyValuePair<Type, StreamIdentifier>(type, null)).ToList();
        }

        //public void AddTypeFilter<TAggregate>(Type type, Guid id) where TAggregate : Aggregate
        //{
        //    Types.Add(new KeyValuePair<Type, StreamIdentifier>(type, new StreamIdentifier(typeof(TAggregate).Name, id)));
        //}

        public EventFilter GetFilter()
        {
            return this;
        }
    }

    public class EventVersion
    {
        public int Version { get; set; }

        public VersionMode VersionMode { get; set; } = VersionMode.Min;
    }

    public enum VersionMode
    {
        Min,
        Max
    }
}
