using EasyEventSourcing.Application;
using System;
using System.Linq;
using EasyEventSourcing.Domain.Messages;
using EasyEventSourcing.Domain.Read.Basket;
using EasyEventSourcing.Domain.Read.Products;

namespace EasyEventSourcing.Ui.Console
{
    internal class Program
    {
        private static readonly Guid ClientId = Guid.Parse("f985f00c-9add-41b7-bd4e-2c331058b74b");

        static void Main(string[] args)
        {
            var app = Bootstrapper.Bootstrap();
            var clientId = app.GetClientId("EasyEventSourcing.Ui.Console");
            var cartId = app.GetCartId(clientId);

            PrintHelp();

            var run = true;
            while (run)
            {
                var input = System.Console.ReadLine();

                //try
                //{
                switch (input)
                {
                    case "1":
                        AddProductToCart(app, cartId);
                        break;
                    case "2":
                        PrintCart(app);
                        break;
                    case "3":
                        Checkout(app, cartId);
                        break;
                    case "4":
                        ConfirmShipping(app, cartId);
                        break;
                    case "5":
                        Pay(app, cartId);
                        break;
                    case "80":
                        app.InitializeProducts();
                        break;
                    case "81":
                        PrintProducts(app);
                        break;
                    case "90":
                        DeleteDb(app);
                        break;
                    case "q":
                        run = false;
                        break;
                    default:
                        PrintHelp();
                        break;
                }
                //}
                //catch (Exception ex)
                //{
                //    System.Console.WriteLine(ex.Message);
                //}
            }

            // Remaining
            //var hasCartBeforeCheckout = app.MongoDb.HasCart(clientId); // MongoDB: not great, is it?
            //var hasCartBeenRemovedAfterCheckout = app.MongoDb.HasCart(clientId);
        }

        private static void PrintHelp()
        {
            System.Console.WriteLine("Please make your choice");
            System.Console.WriteLine("1: Add Product");
            System.Console.WriteLine("2: View Cart");
            System.Console.WriteLine("3: Checkout");
            System.Console.WriteLine("4: ConfirmShipping");
            System.Console.WriteLine("5: Pay");
            System.Console.WriteLine("---");
            System.Console.WriteLine("80: Initialise Products");
            System.Console.WriteLine("81: Print Products");
            System.Console.WriteLine("90: Clear Database");
            System.Console.WriteLine("---");
            System.Console.WriteLine("q: Quit");
        }

        private static void Pay(Bootstrapper.PretendApplication app, Guid orderId)
        {
            app.Send(new PayForOrder { OrderId = orderId });
        }

        private static void DeleteDb(Bootstrapper.PretendApplication app)
        {
            app.DeleteDatabase();

            var cartModel = app.Read<BasketRequest, BasketsReadModel>(new BasketRequest());
            if (!cartModel.HasCart(ClientId))
                app.Send(new CreateNewCart { ClientId = ClientId });
        }

        private static void ConfirmShipping(Bootstrapper.PretendApplication app, Guid orderId)
        {
            app.Send(new ConfirmShippingAddress { OrderId = orderId, Address = "My Home" });
        }

        private static void Checkout(Bootstrapper.PretendApplication app, Guid cartId)
        {
            app.Send(new Checkout { CartId = cartId });
        }

        private static void AddProductToCart(Bootstrapper.PretendApplication app, Guid cartId)
        {
            System.Console.WriteLine("Please specify a product:");
            var productGuid = System.Console.ReadLine();

            System.Console.WriteLine("Please specify a quantity:");
            var productQuantity = System.Console.ReadLine();

            var parsedProductId = Guid.Parse(productGuid);
            var parsedQuantity = int.Parse(productQuantity);

            app.Send(new AddProductToCart { CartId = cartId, ProductId = parsedProductId, Quantity = parsedQuantity });
        }

        private static void PrintCart(Bootstrapper.PretendApplication app)
        {
            var carts = app.Read<BasketRequest, BasketsReadModel>(new BasketRequest());
            var cart = carts.GetCart(ClientId);

            System.Console.WriteLine("-----");
            if (cart.Items.Count > 0)
            {
                System.Console.WriteLine("Your basket:");
                cart.Items.ForEach(PrintShoppingCartItemReadModel);
                System.Console.WriteLine("-----");
            }
            else
            {
                System.Console.WriteLine("Your basket is empty");
            }
        }

        private static void PrintShoppingCartItemReadModel(BasketItemReadModel itemModel)
        {
            System.Console.WriteLine($"ProductID: {itemModel.ProductId} | Description: { itemModel.Description } | Price: {itemModel.ItemPrice} | Quantity: {itemModel.Quantity}");
        }

        private static void PrintProducts(Bootstrapper.PretendApplication app)
        {
            var model = app.Read<ProductsRequest, ProductsReadModel>(new ProductsRequest());

            System.Console.WriteLine("-----");
            if (model.GetAll().Count > 0)
            {
                System.Console.WriteLine("Products:");
                model.GetAll().ToList().ForEach(PrintProducts);
                System.Console.WriteLine("-----");
            }
            else
            {
                System.Console.WriteLine("No products");
            }
        }

        private static void PrintProducts(ProductReadModel product)
        {
            System.Console.WriteLine($"ProductID: {product.ProductId} | Description: { product.Description } | Price: {product.Price}");
        }

    }
}
