using EasyEventSourcing.EventSourcing.Handlers;
using System.Collections.Generic;
using System;
using EasyEventSourcing.Domain.Messages;
using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.Domain.Read.Basket
{
    public class BasketEventHandler
        : IEventHandler<CartCreated>
        , IEventHandler<ProductAddedToCart>
        , IEventHandler<ProductRemovedFromCart>
        , IEventHandler<CartEmptied>
        , IEventHandler<CartCheckedOut>
        , IEventHandler<ProductUpdated>
        , IReadModelCreator<BasketsReadModel>
    {
        public BasketsReadModel Model { get; private set; }

        private readonly IDictionary<Guid, ProductUpdated> productPrices = new Dictionary<Guid, ProductUpdated>();

        public BasketEventHandler()
        {
            this.Model = new BasketsReadModel();
        }

        public void Handle(CartCreated evt)
        {
            Model.CartCreated(evt.ClientId, evt.CartId);
        }

        public void Handle(ProductAddedToCart evt)
        {
            var product = productPrices[evt.ProductId];
            Model.ProductAddedToCart(evt.CartId, evt.ProductId, product.Price, product.Description, evt.Quantity, evt.Timestamp);
        }

        public void Handle(ProductRemovedFromCart evt)
        {
            Model.ProductRemovedFromCart(evt.CartId, evt.ProductId, evt.Quantity, evt.Timestamp);
        }

        public void Handle(CartEmptied evt)
        {
            Model.CartEmptied(evt.CartId, evt.Timestamp);
        }

        public void Handle(CartCheckedOut evt)
        {
            Model.CartCheckedOut(evt.CartId, evt.Timestamp);
        }

        public void Handle(ProductUpdated evt)
        {
            if (productPrices.ContainsKey(evt.ProductId))
            {
                productPrices.Remove(evt.ProductId);
                Model.ProductUpdated(evt.ProductId, evt.Price, evt.Description, evt.Timestamp);
            }

            productPrices.Add(evt.ProductId, evt);
        }
    }
}