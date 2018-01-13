namespace EasyEventSourcing.EventSourcing.ReadModels
{
    public interface IDataReader
    {
        TReadModel Read<TReadModelRequest, TReadModel>(TReadModelRequest request) 
            where TReadModelRequest : IReadModelRequest<TReadModel> 
            where TReadModel : IReadModel;
    }
}