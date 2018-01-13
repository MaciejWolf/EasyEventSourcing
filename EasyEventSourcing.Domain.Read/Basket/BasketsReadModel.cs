using System;
using System.Collections.Generic;
using System.Linq;
using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.Domain.Read.Basket
{
    public class BasketsReadModel : IReadModel
    {
        private readonly IDictionary<Guid, Guid> clientCartMapping = new Dictionary<Guid, Guid>();

        private readonly IDictionary<Guid, BasketReadModel> cartItemsMapping = new Dictionary<Guid, BasketReadModel>();

        #region Model State Queries

        public bool HasCart(Guid clientId)
        {
            return clientCartMapping.ContainsKey(clientId);
        }

        public BasketReadModel GetCart(Guid clientId)
        {
            var cartId = clientCartMapping[clientId];
            return cartItemsMapping[cartId];
        }

        #endregion

        #region Event Replay

        public void CartCreated(Guid clientId, Guid cartId)
        {
            // Overwrite old cart, when new cart happens
            if (!clientCartMapping.ContainsKey(clientId))
            {
                clientCartMapping.Add(clientId, cartId);
            }
            else
            {
                var oldCartId = clientCartMapping[clientId];
                cartItemsMapping.Remove(oldCartId);
                clientCartMapping[clientId] = cartId;
            }

            if (!cartItemsMapping.ContainsKey(clientId))
            {
                cartItemsMapping.Add(cartId, new BasketReadModel { ClientId = clientId, CartId = cartId });
            }
            else
            {
                cartItemsMapping[cartId] = new BasketReadModel { ClientId = clientId, CartId = cartId };
            }
        }

        public void ProductAddedToCart(Guid cartId, Guid productId, decimal price, string description, int quantity, DateTime timestamp)
        {
            var cart = cartItemsMapping[cartId];

            var product = cart.Items.FirstOrDefault(x => x.ProductId == productId);
            if (product != null)
            {
                product.Description = description;
                product.ItemPrice = price;
                product.Quantity += quantity;
                product.TotalPrice = price * product.Quantity;
            }
            else
            {
                cart.Items.Add(new BasketItemReadModel
                {
                    Description = description,
                    ItemPrice = price,
                    ProductId = productId,
                    Quantity = quantity,
                    TotalPrice = price * quantity
                });
            }

            Update(cart);
            cart.Timestamps.Add(timestamp);
            cart.Timestamp = timestamp;
        }

        public void ProductRemovedFromCart(Guid cartId, Guid productId, int quantity, DateTime timestamp)
        {
            var cart = cartItemsMapping[cartId];
            cart.Items
               .Where(item => item.ProductId == productId)
               .ToList()
               .ForEach(item => item.Quantity -= quantity);

            cart.Items.RemoveAll(x => x.Quantity < 1);

            Update(cart);
            cart.Timestamps.Add(timestamp);
            cart.Timestamp = timestamp;
        }

        public void CartEmptied(Guid cartId, DateTime timestamp)
        {
            var cart = cartItemsMapping[cartId];
            cart.Items.Clear();

            Update(cart);
            cart.Timestamps.Add(timestamp);
            cart.Timestamp = timestamp;
        }

        public void CartCheckedOut(Guid cartId, DateTime timestamp)
        {
            var cart = cartItemsMapping[cartId];
            cart.Items.Clear();

            Update(cart);
            cart.Timestamps.Add(timestamp);
            cart.Timestamp = timestamp;
        }

        public void ProductUpdated(Guid productId, decimal price, string description, DateTime timestamp)
        {
            foreach (var cart in cartItemsMapping)
            {
                cart.Value.Items
                    .Where(item => item.ProductId == productId)
                    .ToList()
                    .ForEach(item =>
                        {
                            item.ItemPrice = price;
                            item.Description = description;
                            item.TotalPrice = price * item.Quantity;
                            cart.Value.Timestamps.Add(timestamp);
                            cart.Value.Timestamp = timestamp;
                        });

                Update(cart.Value);
            }
        }

        #endregion

        #region Helpers

        private void Update(BasketReadModel model)
        {
            model.TotalPrice = model.Items.Sum(item => item.TotalPrice);
            model.ItemCount = model.Items.Select(item => item.Quantity).Sum();
        }

        #endregion
    }
}