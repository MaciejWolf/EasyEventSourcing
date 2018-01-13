namespace EasyEventSourcing.EventSourcing.EventProcessing
{
    public interface ICommandHandlerRegistrator
    {
        void Register(ICommandHandlerFactory registration);
    }
}