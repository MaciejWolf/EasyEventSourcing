using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EasyEventSourcing.EventSourcing.EventProcessing;
using EasyEventSourcing.EventSourcing.Events;
using EasyEventSourcing.EventSourcing.Exceptions;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.Persistence;

namespace EasyEventSourcing.Application.Factories
{
    public class CommandHandlerFactory : ICommandHandlerFactory, ICommandDispatcher
    {
        private readonly IEventStore eventStore;

        private readonly Dictionary<Type, IHandler> handlerFactories = new Dictionary<Type, IHandler>();

        public CommandHandlerFactory(IEventStore eventStore)
        {
            this.eventStore = eventStore;

            // Manual loading of events
            var assembly = Assembly.Load(new AssemblyName("EasyEventSourcing.Domain"));
            LoadFromAssembly(assembly);
        }

        public void Send<TCommand>(TCommand command) where TCommand : ICommand
        {
            var handler = Resolve<TCommand>();
            handler.Handle(command);
        }

        private ICommandHandler<TCommand> Resolve<TCommand>() where TCommand : ICommand
        {
            if (handlerFactories.ContainsKey(typeof(TCommand)))
            {
                if (handlerFactories[typeof(TCommand)] is ICommandHandler<TCommand> handler)
                {
                    return handler;
                }
            }
            throw new NoCommandHandlerRegisteredException(typeof(TCommand));
        }

        public void RegisterCommandHandler<TCommandHandler>() where TCommandHandler : IHandler
        {
            // Create instance of command handler
            IRepository TransientRepository() => new Repository(eventStore);
            var commandHandler = (TCommandHandler)Activator.CreateInstance(typeof(TCommandHandler), TransientRepository());

            // work out what it subscribes to
            var types = commandHandler.GetType().GetInterfaces()
                .Where(type => type.GetInterface(typeof(IHandler).FullName) != null)
                .SelectMany(type => type.GetGenericArguments())
                .Distinct()
                .ToArray();

            // Make available
            foreach (var type in types)
            {
                if (!handlerFactories.ContainsKey(type))
                    handlerFactories.Add(type, commandHandler);
            }
        }

        private void LoadFromAssembly(Assembly assembly)
        {
            var types = assembly
                .GetTypes()
                .Where(p => p.IsClass && p.GetInterface(typeof(ICommandHandlerRegistrator).Name) != null)
                .ToList();

            foreach (var handlerType in types)
            {
                var handler = (ICommandHandlerRegistrator)Activator.CreateInstance(handlerType);
                handler.Register(this);
            }
        }
    }
}