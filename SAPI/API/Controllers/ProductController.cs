using API.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;

namespace API.Controllers
{
    [Route("api/product")]
    public class ProductController : BaseController
    {
        private Repositories.Mongo.Product MongoItems { get; set; }
        private Repositories.Mongo.Category MongoCats { get; set; }
        private Repositories.Mongo.SubCategory MongoSubs { get; set; }
        private Repositories.Mongo.Resource MongoRes { get; set; }

        private Repositories.Redis.Product RedisItems { get; set; }

        private IOptionsSnapshot<Redis> CachingOptions { get; set; }

        private Databases.Redis RedisClient { get; set; }

        public ProductController(IOptionsSnapshot<Mongo> options, IOptionsSnapshot<Redis> caching, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor) : base(options, secret, httpContextAccessor)
        {
            CachingOptions = caching;
            RedisClient = new Databases.Redis(CachingOptions);
            RedisItems = new Repositories.Redis.Product(RedisClient);

            MongoItems = new Repositories.Mongo.Product(MongoClient);
            MongoSubs = new Repositories.Mongo.SubCategory(MongoClient);
            MongoCats = new Repositories.Mongo.Category(MongoClient);
            MongoRes = new Repositories.Mongo.Resource(MongoClient);
        }

        [HttpPost("create")]
        public DTO.Messages.Wrapper CreateItem([FromBody] DTO.Messages.Product Product)
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });
            Result.Data = Product;

            if (Product == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Product.Name) || Product.Name.Length < 8)
                {
                    Result.Messages.Add("Name", "can't be empty or shorter than 8 characters");
                }
                if (string.IsNullOrWhiteSpace(Product.Description) || Product.Description.Length < 25)
                {
                    Result.Messages.Add("Description", "can't be empty or shorter than 25 characters");
                }
                if (Product.Price < 0)
                {
                    Result.Messages.Add("Price", "must be not negative number");
                }
                if (Product.Images.Count < 1)
                {
                    Result.Messages.Add("Images", "please specify at least 1 image for this product");
                }
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var NewProduct = new DTO.Databases.Product
            {
                Name = Product.Name,
                Description = Product.Description,
                Price = Product.Price,
                Images = Product.Images,
                Categories = Product.Categories,
                SubCategories = Product.SubCategories,
                Owner = Token.sub
            };
            var Inserted = MongoItems.Save(NewProduct);

            Result.Data = NewProduct;
            Result.Messages.Add("Save", Inserted ? "Ok" : "Failed");
            return Result;
        }

        [HttpPost("update")]
        public DTO.Messages.Wrapper UpdateOne([FromBody] DTO.Databases.Product Product)
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });

            if (Product == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Product.Id))
                {
                    Result.Messages.Add("Id", "can't be empty");
                }
                if (string.IsNullOrWhiteSpace(Product.Name) || Product.Name.Length < 8)
                {
                    Result.Messages.Add("Name", "can't be empty or shorter than 8 characters");
                }
                if (string.IsNullOrWhiteSpace(Product.Description) || Product.Description.Length < 25)
                {
                    Result.Messages.Add("Description", "can't be empty or shorter than 25 characters");
                }
                if (Product.Price < 0)
                {
                    Result.Messages.Add("Price", "must be not negative number");
                }
                if (Product.Images.Count < 1)
                {
                    Result.Messages.Add("Images", "please specify at least 1 image for this product");
                }
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var Inserted = MongoItems.Save(Product);
            Result.Data = Product;
            Result.Messages.Add("Update", Inserted ? "Ok" : "Failed");
            return Result;
        }

        [HttpGet("all")]
        [HttpPost("all")]
        public DTO.Messages.Wrapper GetAll()
        {
            return null;
        }

        [HttpGet("onsale")]
        [HttpPost("onsale")]
        public DTO.Messages.Wrapper GetOnSale()
        {
            return null;
        }
    }
}