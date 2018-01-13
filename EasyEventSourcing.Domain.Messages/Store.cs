using System;
using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.Domain.Messages
{
    public class CreateNewCart : ICommand
    {
        public Guid ClientId { get; set; }
    }

    public class CartCreated : Event
    {
        public Guid CartId { get; set; }

        public Guid ClientId { get; set; }
    }

    public class AddProductToCart : ICommand
    {
        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class ProductAddedToCart : Event
    {
        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class RemoveProductFromCart : ICommand
    {
        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class ProductRemovedFromCart : Event
    {
        public Guid CartId { get; set; }

        public Guid ProductId { get; set; }

        public int Quantity { get; set; }
    }

    public class EmptyCart : ICommand
    {
        public Guid CartId { get; set; }
    }

    public class CartEmptied : Event
    {
        public Guid CartId { get; set; }
    }

    public class Checkout : ICommand
    {
        public Guid CartId { get; set; }
    }

    public class CartCheckedOut : Event
    {
        public Guid CartId { get; set; }
    }
}