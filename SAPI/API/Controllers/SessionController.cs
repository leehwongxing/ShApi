using API.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("cart")]
    public class SessionController : BaseController
    {
        private Repositories.Mongo.Token SessionRepo { get; set; }

        private Repositories.Mongo.Order OrderRepo { get; set; }

        private Repositories.Mongo.Product ProductRepo { get; set; }

        public SessionController(IOptionsSnapshot<Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor) : base(options, secret, httpContextAccessor)
        {
            SessionRepo = new Repositories.Mongo.Token(MongoClient);
            OrderRepo = new Repositories.Mongo.Order(MongoClient);
            ProductRepo = new Repositories.Mongo.Product(MongoClient);
        }

        [HttpGet("all")]
        [HttpPost("all")]
        public DTO.Messages.Wrapper GetAll()
        {
            var Result = AuthorizeResponse();
            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var Session = SessionRepo.GetOne(Token.jti);

            var Cart = new Dictionary<DTO.Projection.Recommendation, int>();
            var Products = ProductRepo.QueryableCollection
                    .Where(x => Session.Cart.ContainsKey(x.Id))
                    .Select(x => new DTO.Projection.Recommendation
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        Images = x.Images
                    });

            foreach (var Product in Products)
            {
                Session.Cart.TryGetValue(Product.Id, out int Quantity);
                Cart.Add(Product, Quantity);
            }

            Result.Data = Cart;
            return Result;
        }

        [HttpPost("add")]
        public DTO.Messages.Wrapper AddOne(DTO.Messages.AddToCart Adding = null)
        {
            var Result = AuthorizeResponse();
            if (Adding == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Adding.ProductId))
                {
                    Result.Messages.Add("ProductId", "can't be empty");
                }
                else
                {
                    var Product = ProductRepo.GetOne(Adding.ProductId);
                    if (Product == null)
                    {
                        Result.Messages.Add("ProductId", "is invalid");
                    }
                    else if (Product.Scored() < 0)
                    {
                        Result.Messages.Add("Product", "is unavailable due to Promotions");
                    }
                }

                if (Adding.Quantity < 1)
                {
                    Result.Messages.Add("Quantity", "must be greater than 0");
                }
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var Session = SessionRepo.GetOne(Token.jti);
            Session.Cart.Add(Adding.ProductId, Adding.Quantity);
            SessionRepo.Save(Session);

            Result.Data = Adding;
            return Result;
        }

        [HttpPost("remove")]
        public DTO.Messages.Wrapper RemoveOne(DTO.Messages.AddToCart ToBeRemoved = null)
        {
            var Result = AuthorizeResponse();

            if (ToBeRemoved == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            if (string.IsNullOrWhiteSpace(ToBeRemoved.ProductId))
            {
                Result.Messages.Add("ProductId", "can't be empty");
            }
            if (ToBeRemoved.Quantity == 0)
            {
                Result.Messages.Add("Quantity", "can't be Zero (0)");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var Session = SessionRepo.GetOne(Token.jti);
            var Removed = false;
            if (ToBeRemoved.Quantity < 0)
            {
                Session.Cart.Remove(ToBeRemoved.ProductId);
                Removed = true;
            }
            else
            {
                Session.Cart.TryGetValue(ToBeRemoved.ProductId, out int Left);
                if (Left < 1 || Left - ToBeRemoved.Quantity < 1)
                {
                    Session.Cart.Remove(ToBeRemoved.ProductId);
                    Removed = true;
                }
                else
                {
                    Session.Cart[ToBeRemoved.ProductId] = (Left - ToBeRemoved.Quantity);
                    Removed = false;
                }
            }
            SessionRepo.Save(Session);

            Result.Messages.Add("ItemStatus", Removed ? "Ok" : "Reduced");
            Result.Data = ToBeRemoved;
            return Result;
        }

        [HttpPost("address/delivery")]
        public DTO.Messages.Wrapper AddDeliveryAddress(DTO.Messages.TemporaryAddress address = null)
        {
            var Result = AuthorizeResponse();

            if (address == null)
            {
                Result.Messages.Add("PostBody", "It mustn't be empty");
            }
            if (string.IsNullOrWhiteSpace(address.Location))
            {
                Result.Messages.Add("Location", "It mustn't be empty");
            }
            if (string.IsNullOrWhiteSpace(address.Recipent))
            {
                Result.Messages.Add("Recipent", "It mustn't be empty");
            }
            if (string.IsNullOrWhiteSpace(address.Phone))
            {
                Result.Messages.Add("Phone", "It mustn't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var Session = SessionRepo.GetOne(Token.jti);
            Session.DeliveryAddress = address;
            var Saved = SessionRepo.Save(Session);

            Result.Messages.Add("DeliveryAddress", (Saved ? "Ok" : "Fail"));
            return Result;
        }

        [HttpPost("address/payment")]
        public DTO.Messages.Wrapper AddPaymentAddress(DTO.Messages.TemporaryAddress address = null)
        {
            var Result = AuthorizeResponse();

            if (address == null)
            {
                Result.Messages.Add("PostBody", "It mustn't be empty");
            }
            if (string.IsNullOrWhiteSpace(address.Location))
            {
                Result.Messages.Add("Location", "It mustn't be empty");
            }
            if (string.IsNullOrWhiteSpace(address.Recipent))
            {
                Result.Messages.Add("Recipent", "It mustn't be empty");
            }
            if (string.IsNullOrWhiteSpace(address.Phone))
            {
                Result.Messages.Add("Phone", "It mustn't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var Session = SessionRepo.GetOne(Token.jti);
            Session.DeliveryAddress = address;
            var Saved = SessionRepo.Save(Session);

            Result.Messages.Add("PaymentAddress", (Saved ? "Ok" : "Fail"));
            return Result;
        }

        [HttpPost("checkout")]
        public DTO.Messages.Wrapper MakeOrder([FromBody] DTO.Messages.CheckOutCart Data = null)
        {
            var Result = AuthorizeResponse();
            var Session = SessionRepo.GetOne(Token.jti);

            if (Data == null)
            {
                Data = new DTO.Messages.CheckOutCart();
            }
            if (Session.Cart.Count == 0)
            {
                Result.Messages.Add("Cart", "is empty, please choose something before check out");
            }
            if (Session.DeliveryAddress == null)
            {
                Result.Messages.Add("DeliveryAddress", "is not set");
            }
            if (Session.PaymentAddress == null)
            {
                Result.Messages.Add("PaymentAddress", "is not set");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
                return Result;
            }

            var Cart = new Dictionary<DTO.Projection.Recommendation, int>();
            var Products = ProductRepo.QueryableCollection
                    .Where(x => Session.Cart.ContainsKey(x.Id))
                    .Select(x => new DTO.Projection.Recommendation
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Price = x.Price,
                        Images = x.Images
                    });

            foreach (var Product in Products)
            {
                Session.Cart.TryGetValue(Product.Id, out int Quantity);
                Cart.Add(Product, Quantity);
            }

            var Order = new DTO.Databases.Order
            {
                Note = Data.Note,
                OrdererId = Token.sub,
                Delivery = Session.DeliveryAddress,
                Payment = Session.PaymentAddress,
                Ordered = Cart
            };
            OrderRepo.Save(Order);

            Session.Cart = new Dictionary<string, int>();
            SessionRepo.Save(Session);

            return Result;
        }
    }
}