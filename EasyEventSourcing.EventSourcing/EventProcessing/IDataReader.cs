using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.EventSourcing.EventProcessing
{
    public interface IDataReader
    {
        TReadModel Read<TReadModelRequest, TReadModel>(TReadModelRequest request) 
            where TReadModelRequest : IReadModelRequest<TReadModel> 
            where TReadModel : IReadModel;
    }
}