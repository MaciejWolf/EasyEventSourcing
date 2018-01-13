using System;
using EasyEventSourcing.Domain.Messages;
using EasyEventSourcing.EventSourcing.Domain;

namespace EasyEventSourcing.Domain.Products
{
    public class Product : Aggregate
    {
        protected override void RegisterAppliers()
        {
            this.RegisterApplier<ProductUpdated>(this.Apply);
        }

        private Guid productId;
        private string description;
        private decimal price;

        public static Product Create(Guid productId, string description, decimal price)
        {
            return new Product(productId, description, price);
        }

        public Product()
        {
        }

        private Product(Guid productId, string description, decimal price)
        {
            this.ApplyChanges(new ProductUpdated { ProductId = productId, Description = description, Price = price });
        }

        private void Apply(ProductUpdated evt)
        {
            this.productId = evt.ProductId;
            this.description = evt.Description;
            this.price = evt.Price;
        }

        public void UpdateProduct(Guid productId, string description, decimal price)
        {
            this.productId = productId;
            this.description = description;
            this.price = price;

            this.ApplyChanges(new ProductUpdated { ProductId = this.productId, Description = this.description, Price = this.price });
        }
    }
}