using System;
using System.Collections.Generic;
using System.Web.Http;
using EasyEventSourcing.Domain.Messages;
using EasyEventSourcing.Domain.Read.Products;

namespace EasyEventSourcing.Ui.Api.Controllers
{
    public class ProductsController : BaseController
    {
        [HttpGet]
        public ICollection<ProductReadModel> Get()
        {
            var products = App.Read<ProductsRequest, ProductsReadModel>(new ProductsRequest());
            return products.GetAll();
        }

        [HttpGet]
        public ProductReadModel Get(Guid id)
        {
            var products = App.Read<ProductsRequest, ProductsReadModel>(new ProductsRequest());
            return products.GetProduct(id);
        }

        [HttpPost]
        public void Post()
        {
            App.InitializeProducts();
        }

        [HttpPut]
        public void Put(ProductReadModel product)
        {
            App.Send(new UpdateProduct { ProductId = product.ProductId, Description = product.Description, Price = product.Price });
        }

        [HttpDelete]
        public void Delete(ProductReadModel product)
        {
            // Nope, hasn't been implemented
        }
    }
}
