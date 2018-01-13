namespace EasyEventSourcing.EventSourcing.Domain
{
    public abstract class Aggregate : EventStream
    {
        public int Version { get; set; }
    }
}
