using System;
using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.Domain.Messages
{
    public class ShippingItem
    {
        public Guid ItemId { get; set; }
    }

    public class PaymentConfirmed : Event
    {
        public Guid OrderId { get; set; }
    }

    public class StartedShippingProcess : Event
    {
        public Guid OrderId { get; set; }
    }

    public class AddressConfirmed : Event
    {
        public Guid OrderId { get; set; }
    }

    public class OrderDelivered : Event
    {
        public Guid OrderId { get; set; }
    }
}
