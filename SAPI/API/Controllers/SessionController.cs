using API.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/cart")]
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

        public DTO.Messages.Wrapper RemoveOne()
        {
            return null;
        }

        public DTO.Messages.Wrapper AddDeliveryAddress()
        {
            return null;
        }

        public DTO.Messages.Wrapper AddPaymentAddress()
        {
            return null;
        }

        public DTO.Messages.Wrapper MakeOrder()
        {
            return null;
        }
    }
}