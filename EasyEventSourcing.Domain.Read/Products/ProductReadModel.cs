using System;

namespace EasyEventSourcing.Domain.Read.Products
{
    public class ProductReadModel
    {
        public Guid ProductId { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }
    }
}
