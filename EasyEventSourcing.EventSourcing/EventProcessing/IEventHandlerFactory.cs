using System.Collections.Generic;
using EasyEventSourcing.EventSourcing.Events;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.EventSourcing.EventProcessing
{
    public interface IEventHandlerFactory
    {
        IEnumerable<IEventHandler<TEvent>> ResolveEventHandler<TEvent>(TEvent evt) where TEvent : IEvent;

        IReadModelCreator<TReadModel> ResolveReadModel<TReadModel>() where TReadModel : IReadModel;

        void RegisterHandler<TReadModelCreator, TReadModel>() where TReadModelCreator : IReadModelCreator<TReadModel>;
    }
}