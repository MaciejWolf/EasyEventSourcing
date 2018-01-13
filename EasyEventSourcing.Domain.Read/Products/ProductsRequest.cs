using System;
using EasyEventSourcing.EventSourcing.Persistence;
using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.Domain.Read.Products
{
    public class ProductsRequest : IReadModelRequest<ProductsReadModel>
    {
        public DateTime? Timestamp { get; set; }

        public EventFilter Filter => new EventFilter { Timestamp = Timestamp };
    }
}
