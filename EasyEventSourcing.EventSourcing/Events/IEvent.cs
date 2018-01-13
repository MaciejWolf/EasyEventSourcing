using System;

namespace EasyEventSourcing.EventSourcing.Events
{
    public interface IEvent
    {
        DateTime Timestamp { get; set; }

        int Version { get; set; }
    }

    public class Event : IEvent
    {
        public DateTime Timestamp { get; set; } = DateTime.MinValue;

        public int Version { get; set; }
    }
}
