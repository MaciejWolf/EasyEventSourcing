namespace EasyEventSourcing.EventSourcing.EventProcessing
{
    public interface IEventHandlerRegistrator
    {
        void Register(IEventHandlerFactory registration );
    }
}