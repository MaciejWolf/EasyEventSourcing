using System;

namespace EasyEventSourcing.Domain.Read.Basket
{
    public class BasketItemReadModel
    {
        public Guid ProductId { get; set; }

        public string Description { get; set; }

        public decimal ItemPrice { get; set; }

        public int Quantity { get; set;  }

        public decimal TotalPrice { get; set; }
    }
}