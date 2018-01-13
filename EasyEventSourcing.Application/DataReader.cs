using System.Linq;
using EasyEventSourcing.EventSourcing.EventProcessing;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.Persistence;
using EasyEventSourcing.EventSourcing.ReadModels;
using IDataReader = EasyEventSourcing.EventSourcing.EventProcessing.IDataReader;

namespace EasyEventSourcing.Application
{
    public class DataReader : IDataReader
    {
        private readonly IEventStore eventStore;
        private readonly IEventHandlerFactory eventHandlerFactory;

        public DataReader(IEventStore eventStore, IEventHandlerFactory eventHandlerFactory)
        {
            this.eventStore = eventStore;
            this.eventHandlerFactory = eventHandlerFactory;
        }

        public TReadModel Read<TReadModelRequest, TReadModel>(TReadModelRequest request)
            where TReadModelRequest : IReadModelRequest<TReadModel>
            where TReadModel : IReadModel
        {
            // TODO cache the read model
            var eventHandler = eventHandlerFactory.ResolveReadModel<TReadModel>();

            // Workout what events the handler implements
            var types = eventHandler.GetType().GetInterfaces()
                .Where(type => type.GetInterface(typeof(IHandler).FullName) != null)
                .SelectMany(type => type.GetGenericArguments())
                .Distinct()
                .ToArray();

            var eventFilter = request.Filter;
            eventFilter.AddTypeFilter(types);

            // Replay all events in the database
            var events = eventStore.GetEvents(eventFilter);
            foreach (var evt in events)
            {
                var eventType = evt.GetType();
                var handleMethod = eventHandler.GetType().GetMethod("Handle", new[] { eventType });
                handleMethod.Invoke(eventHandler, new object[] { evt });
            }

            return eventHandler.Model;
        }
    }
}
