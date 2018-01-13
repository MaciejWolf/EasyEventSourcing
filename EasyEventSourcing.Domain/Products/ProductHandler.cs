using EasyEventSourcing.Domain.Messages;
using EasyEventSourcing.EventSourcing.Handlers;
using EasyEventSourcing.EventSourcing.Persistence;

namespace EasyEventSourcing.Domain.Products
{
    public class ProductHandler
        : ICommandHandler<UpdateProduct>
    {
        private readonly IRepository repository;

        public ProductHandler(IRepository repository)
        {
            this.repository = repository;
        }

        public void Handle(UpdateProduct cmd)
        {
            var product = this.repository.GetById<Product>(cmd.ProductId);
            product.UpdateProduct(cmd.ProductId, cmd.Description, cmd.Price);
            this.repository.Save(product);
        }
    }
}