using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyEventSourcing.EventSourcing.EventProcessing;
using EasyEventSourcing.EventSourcing.Events;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.Persistence;
using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.Application.Factories
{
    public class EventHandlerFactory : IEventHandlerFactory
    {
        private readonly Dictionary<Type, List<IHandler>> handlerFactories = new Dictionary<Type, List<IHandler>>();
        private readonly Dictionary<Type, Type> readModelHandlers = new Dictionary<Type, Type>();

        private ICommandDispatcher dispatcher; // TODO: Remove as it was used for the SAGA?
        private IEventStore eventStore;

        public EventHandlerFactory(IEventStore eventStore, ICommandDispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
            this.eventStore = eventStore;

            var assembly = Assembly.Load(new AssemblyName("EasyEventSourcing.Domain.Read"));
            LoadFromAssembly(assembly);
        }

        public IEnumerable<IEventHandler<TEvent>> ResolveEventHandler<TEvent>(TEvent evt) where TEvent : IEvent
        {
            var evtType = evt.GetType();
            if (this.handlerFactories.ContainsKey(evtType))
            {
                var factories = this.handlerFactories[evtType];
                return factories.Select(h => (IEventHandler<TEvent>)h);
            }
            return new List<IEventHandler<TEvent>>();
        }

        public IReadModelCreator<TReadModel> ResolveReadModel<TReadModel>() where TReadModel : IReadModel
        {
            var handlerClass = readModelHandlers[typeof(TReadModel)];

            // TODO Param needs to filter the events, where does that happen?

            //var readModel = new TReadModel(); // No need to pass in the read model I believe
            var eventHandler = (IReadModelCreator<TReadModel>)Activator.CreateInstance(handlerClass);

            return eventHandler;
        }

        public void RegisterHandler<TReadModelCreator, TReadModel>() where TReadModelCreator : IReadModelCreator<TReadModel>
        {
            var handler = typeof(TReadModelCreator);
            var readModel = typeof(TReadModel);

            readModelHandlers.Add(readModel, handler);
        }

        private void LoadFromAssembly(Assembly assembly)
        {
            var types = assembly
                .GetTypes()
                .Where(p => p.IsClass && p.GetInterface(typeof(IEventHandlerRegistrator).Name) != null)
                .ToList();

            foreach (var handlerType in types)
            {
                var handler = (IEventHandlerRegistrator)Activator.CreateInstance(handlerType);
                handler.Register(this);
            }
        }
    }
}