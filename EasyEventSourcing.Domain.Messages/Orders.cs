using System;
using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.Domain.Messages
{
    public class OrderCreated : Event
    {
        public Guid OrderId { get; set; }

        public Guid ClientId { get; set; }

        public OrderItem[] Items { get; set; }
    }

    public class OrderItem
    {
        public Guid ProductId { get; set; }

        public decimal Price { get; set; }
    }

    public class PayForOrder : ICommand
    {
        public Guid OrderId { get; set; }

        public decimal Price { get; set; }
    }

    public class PaymentReceived : Event
    {
        public Guid OrderId { get; set; }
    }

    public class ConfirmShippingAddress : ICommand
    {
        public Guid OrderId { get; set; }

        public string Address { get; set; }
    }

    public class ShippingAddressConfirmed : Event
    {
        public Guid OrderId { get; set; }

        public string Address { get; set; }
    }

    public class CompleteOrder : ICommand
    {
        public Guid OrderId { get; set; }
    }

    public class OrderCompleted : Event
    {
        public Guid OrderId { get; set; }
    }
}