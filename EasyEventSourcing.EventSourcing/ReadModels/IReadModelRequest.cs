using EasyEventSourcing.EventSourcing.Persistence;

namespace EasyEventSourcing.EventSourcing.ReadModels
{
    public interface IReadModelRequest<TReadModel> where TReadModel : IReadModel
    {
        EventFilter Filter { get; }
    }
}