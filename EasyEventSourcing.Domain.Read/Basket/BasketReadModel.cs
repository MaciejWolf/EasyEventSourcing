using System;
using System.Collections.Generic;
using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.Domain.Read.Basket
{
    public class BasketReadModel : IReadModel
    {
        public BasketReadModel()
        {
            Items = new List<BasketItemReadModel>();
            Timestamps = new List<DateTime>();
        }

        public bool HasItems => Items.Count > 0;

        public DateTime? Timestamp { get; set; } // TODO base class?

        public List<BasketItemReadModel> Items { get; }

        public ICollection<DateTime> Timestamps { get; }

        public Guid CartId { get; set; }

        public Guid ClientId { get; set; }

        public decimal TotalPrice { get; set; }

        public int ItemCount { get; set; }
    }
}