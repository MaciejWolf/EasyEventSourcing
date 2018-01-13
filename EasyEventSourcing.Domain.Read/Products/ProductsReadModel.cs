using System;
using System.Collections.Generic;
using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.Domain.Read.Products
{
    public class ProductsReadModel : IReadModel
    {
        private readonly IDictionary<Guid, ProductReadModel> products = new Dictionary<Guid, ProductReadModel>();

        #region Model State Queries

        public bool HasProduct(Guid productId)
        {
            return products.ContainsKey(productId);
        }

        public ProductReadModel GetProduct(Guid productId)
        {
            return products[productId];
        }

        public ICollection<ProductReadModel> GetAll()
        {
            return products.Values;
        }

        #endregion

        #region Event Replay

        public void ProductUpdated(Guid productId, string description, decimal price)
        {
            if (products.ContainsKey(productId))
            {
                var product = products[productId];
                product.Description = description;
                product.Price = price;
            }
            else
            {
                var newProduct = new ProductReadModel
                {
                    ProductId = productId,
                    Description = description,
                    Price = price
                };
                products.Add(productId, newProduct);
            }
        }

        #endregion 
    }
}