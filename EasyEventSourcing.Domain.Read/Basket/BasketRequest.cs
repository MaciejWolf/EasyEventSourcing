using EasyEventSourcing.EventSourcing.Persistence;
using System;
using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.Domain.Read.Basket
{
    public class BasketRequest : IReadModelRequest<BasketsReadModel>
    {
        public DateTime? Timestamp { get; set; }

        public EventFilter Filter => new EventFilter { Timestamp = Timestamp };
    }
}
