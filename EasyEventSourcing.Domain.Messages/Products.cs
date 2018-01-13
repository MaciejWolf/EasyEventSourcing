using System;
using EasyEventSourcing.EventSourcing.Events;

namespace EasyEventSourcing.Domain.Messages
{
    public class ProductUpdated : Event
    {
        public Guid ProductId { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }

    public class UpdateProduct : ICommand
    {
        public Guid ProductId { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}