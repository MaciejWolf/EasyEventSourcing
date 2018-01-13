using System;
using System.Collections.Generic;
using EasyEventSourcing.Domain.Orders;
using EasyEventSourcing.EventSourcing.Domain;
using System.Linq;
using EasyEventSourcing.Domain.Messages;

namespace EasyEventSourcing.Domain.Store
{
    public class ShoppingCart : Aggregate
    {
        public ShoppingCart() { }

        protected override void RegisterAppliers()
        {
            this.RegisterApplier<CartCreated>(this.Apply);
            this.RegisterApplier<ProductAddedToCart>(this.Apply);
            this.RegisterApplier<ProductRemovedFromCart>(this.Apply);
            this.RegisterApplier<CartEmptied>(this.Apply);
            this.RegisterApplier<CartCheckedOut>(this.Apply);
        }

        private readonly Dictionary<Guid, Decimal> products = new Dictionary<Guid, decimal>();
        private bool checkedOut;
        private Guid clientId;

        private ShoppingCart(Guid cartId, Guid customerId)
        {
            this.ApplyChanges(new CartCreated { CartId = cartId, ClientId = customerId });
        }

        public static ShoppingCart Create(Guid cartId, Guid customerId)
        {
            return new ShoppingCart(cartId, customerId);
        }

        private void Apply(CartCreated evt)
        {
            id = evt.CartId;
            clientId = evt.ClientId;
        }

        public void AddProduct(Guid productId, int quantity)
        {
            if (checkedOut)
            {
                throw new CartAlreadyCheckedOutException();
            }

            ApplyChanges(new ProductAddedToCart { CartId = this.id, ProductId = productId, Quantity = quantity });
        }

        private void Apply(ProductAddedToCart evt)
        {
            if (!products.ContainsKey(evt.ProductId))
                products.Add(evt.ProductId, evt.Quantity);
            else
                products[evt.ProductId] += evt.Quantity;
        }

        public void RemoveProduct(Guid productId, int quantity)
        {
            if (checkedOut)
            {
                throw new CartAlreadyCheckedOutException();
            }
            if (products.ContainsKey(productId))
            {
                this.ApplyChanges(new ProductRemovedFromCart { CartId = this.id, ProductId = productId, Quantity = quantity });
            }
        }

        private void Apply(ProductRemovedFromCart evt)
        {
            products.Remove(evt.ProductId);
        }

        public void Empty()
        {
            if (checkedOut)
            {
                throw new CartAlreadyCheckedOutException();
            }
            this.ApplyChanges(new CartEmptied { CartId = this.id });
        }

        private void Apply(CartEmptied evt)
        {
            products.Clear();
        }

        public EventStream Checkout()
        {
            if (this.products.Count == 0)
            {
                throw new CannotCheckoutEmptyCartException();
            }
            this.ApplyChanges(new CartCheckedOut { CartId = this.id });
            return Order.Create(this.id, this.clientId, this.products.Select(x => new OrderItem { ProductId = x.Key, Price = x.Value }));
        }

        private void Apply(CartCheckedOut evt)
        {
            this.checkedOut = true;
        }
    }
}
