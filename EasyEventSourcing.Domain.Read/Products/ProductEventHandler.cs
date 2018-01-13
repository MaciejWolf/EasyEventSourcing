using System;
using System.Collections.Generic;
using EasyEventSourcing.Domain.Messages;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.ReadModels;

namespace EasyEventSourcing.Domain.Read.Products
{
    public class ProductEventHandler : IEventHandler<ProductUpdated>, IReadModelCreator<ProductsReadModel>
    {
        public ProductsReadModel Model { get; }

        public ProductEventHandler()
        {
            this.Model = new ProductsReadModel();
        }

        public void Handle(ProductUpdated evt)
        {
            Model.ProductUpdated(evt.ProductId, evt.Description, evt.Price);
        }
    }
}