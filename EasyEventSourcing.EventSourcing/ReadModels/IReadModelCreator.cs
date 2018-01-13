namespace EasyEventSourcing.EventSourcing.ReadModels
{
    public interface IReadModelCreator<TReadModel>
    {
        TReadModel Model { get; }
    }
}