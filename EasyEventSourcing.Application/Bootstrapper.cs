using System;
using System.Collections.Generic;
using EasyEventSourcing.Application.Factories;
using EasyEventSourcing.Application.InMemoryStore;
using EasyEventSourcing.Domain.Messages;
using EasyEventSourcing.EventSourcing.EventProcessing;
using EasyEventSourcing.EventSourcing.Events;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.Persistence;
using EasyEventSourcing.EventSourcing.ReadModels;
using IDataReader = EasyEventSourcing.EventSourcing.EventProcessing.IDataReader;
using System.Security.Cryptography;
using System.Text;
using EasyEventSourcing.Data.EventStore;
using EasyEventSourcing.Domain.Read.Basket;

namespace EasyEventSourcing.Application
{
    public class Bootstrapper
    {
        private static PretendApplication app;
        private static readonly IDictionary<Guid, Guid> ClientCartIdMapping = new Dictionary<Guid, Guid>();

        public static PretendApplication Bootstrap(bool deleteDatabase = false)
        {
            if (app != null)
                return app;

            //var store = new MongoDbEventStore(new TypeResolver()); // MongoDb Store
            var store = new InMemoryEventStore(); // In-Memory Event Store

            if (deleteDatabase)
                store.DeleteDatabase();

            var handlerFactory = new CommandHandlerFactory(store);
            var eventHandlerFactory = new EventHandlerFactory(store, handlerFactory);
            var eventDispatcher = new EventDispatcher(eventHandlerFactory); // TODO Merge also EventDispatcher and EventHandlerFactory?
            var eventProcessor = new EventProcessor(store, eventDispatcher); // TODO: what that here for?
            var dataReader = new DataReader(store, eventHandlerFactory);

            app = new PretendApplication(handlerFactory, store, dataReader);
            return app;
        }

        public class PretendApplication
        {
            private readonly IEventStore store;
            private readonly IDataReader dataReader;
            private readonly ICommandDispatcher dispatcher;

            public PretendApplication(ICommandDispatcher dispatcher, IEventStore store, IDataReader dataReader)
            {
                this.dispatcher = dispatcher;
                this.store = store;
                this.dataReader = dataReader;
            }

            public void Send<TCommand>(TCommand cmd) where TCommand : ICommand
            {
                dispatcher.Send(cmd);
            }

            public TReadModel Read<TReadModelRequest, TReadModel>(TReadModelRequest request)
                where TReadModelRequest : IReadModelRequest<TReadModel>
                where TReadModel : IReadModel
            {
                return dataReader.Read<TReadModelRequest, TReadModel>(request);
            }

            public void DeleteDatabase()
            {
                store.DeleteDatabase();
            }

            public void InitializeProducts()
            {
                var ran = new Random();
                for (var i = 1; i <= 20; i++)
                {
                    var price = (decimal)Math.Round((ran.NextDouble() + i) * 10, 2);
                    Send(new UpdateProduct { ProductId = Guid.NewGuid(), Description = $"Product {i}", Price = price });
                }
            }

            public Guid GetClientId(string value)
            {
                var md5Hasher = MD5.Create();
                var data = md5Hasher.ComputeHash(Encoding.Default.GetBytes(value));
                return new Guid(data);
            }

            private readonly Object thisLock = new Object();
            public Guid GetCartId(Guid clientId)
            {
                lock (thisLock)
                {
                    if (ClientCartIdMapping.ContainsKey(clientId))
                        return ClientCartIdMapping[clientId];

                    var cartModel = app.Read<BasketRequest, BasketsReadModel>(new BasketRequest());
                    if (cartModel.HasCart(clientId))
                    {
                        var cartId = cartModel.GetCart(clientId).CartId;
                        ClientCartIdMapping.Add(clientId, cartId);
                        return cartId;
                    }

                    app.Send(new CreateNewCart { ClientId = clientId });
                    cartModel = app.Read<BasketRequest, BasketsReadModel>(new BasketRequest());
                    var newCartId = cartModel.GetCart(clientId).CartId;

                    if (ClientCartIdMapping.ContainsKey(clientId))
                        ClientCartIdMapping.Add(clientId, newCartId);

                    return newCartId;
                }
            }
        }

        public class TypeResolver : ITypeResolver
        {
            public Type Lookup(string typeName)
            {
                return Type.GetType(typeName);
            }
        }
    }
}
