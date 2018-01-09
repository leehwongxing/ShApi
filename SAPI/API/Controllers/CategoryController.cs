using API.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("category")]
    public class CategoryController : BaseController
    {
        private Repositories.Mongo.Category CatRepo { get; set; }

        private Repositories.Mongo.SubCategory SubRepo { get; set; }

        public CategoryController(IOptionsSnapshot<Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor) : base(options, secret, httpContextAccessor)
        {
            CatRepo = new Repositories.Mongo.Category(MongoClient);
            SubRepo = new Repositories.Mongo.SubCategory(MongoClient);
        }

        [HttpGet("all")]
        public DTO.Messages.Wrapper GetAll()
        {
            var Result = new DTO.Messages.Wrapper
            {
                Data = CatRepo.QueryableCollection.Select(Cat => new DTO.Projection.Category { Name = Cat.Name, Id = Cat.Id })
            };

            return Result;
        }

        [HttpPost("more")]
        public DTO.Messages.Wrapper SaveMore([FromBody] HashSet<DTO.Messages.Category> Cats)
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });

            if (Cats == null || Cats.Count == 0)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
                return Result;
            }

            var Saving = new HashSet<DTO.Databases.Category>();
            foreach (var Cat in Cats)
            {
                Saving.Add(new DTO.Databases.Category
                {
                    Name = Cat.Name,
                    Owner = Token.sub
                });
            }

            try
            {
                CatRepo.Collection.InsertMany(Saving);
            }
            catch (Exception)
            {
            }

            Result.Data = Saving;
            return Result;
        }

        [HttpPost("one")]
        public DTO.Messages.Wrapper SaveOne([FromBody]DTO.Messages.Category Cat)
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });

            if (Cat == null || string.IsNullOrWhiteSpace(Cat.Name))
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
                return Result;
            }
            var Saving = new DTO.Databases.Category
            {
                Name = Cat.Name,
                Owner = Token.sub
            };
            Result.Messages.Add("Update", CatRepo.Save(Saving) ? "Ok" : "Failed");
            Result.Data = Saving;
            return Result;
        }

        [HttpGet("sub/{id}")]
        public DTO.Messages.Wrapper GetChilds(string id = "")
        {
            var Result = new DTO.Messages.Wrapper();

            if (string.IsNullOrWhiteSpace(id))
            {
                Result.Messages.Add("Parameter", "ID can't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
                return Result;
            }

            var Query = SubRepo.QueryableCollection.Where(x => x.ParentId == id);
            if (Query.Count() == 0)
            {
                Result.Status = "Not Found";
                Result.Code = 404;
            }
            else
            {
                Result.Data = Query.Select(Sub => new DTO.Projection.SubCategory { Name = Sub.Name, Id = Sub.Id, Parent = Sub.ParentId });
            }
            return Result;
        }

        [HttpPost("sub/{id}/more")]
        public DTO.Messages.Wrapper SaveMoreSubs([FromBody] HashSet<DTO.Messages.Category> Cats, string id)
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });
            var Category = CatRepo.GetOne(id);

            if (Cats == null || Cats.Count == 0)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            if (Category == null)
            {
                Result.Messages.Add("Parameter", "ID can't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
                return Result;
            }
            var Saving = new HashSet<DTO.Databases.SubCategory>();
            foreach (var Cat in Cats)
            {
                Saving.Add(new DTO.Databases.SubCategory
                {
                    Name = Cat.Name,
                    Owner = Token.sub,
                    ParentId = Category.Id
                });
            }

            try
            {
                SubRepo.Collection.InsertMany(Saving);
            }
            catch (Exception)
            {
            }

            Result.Data = Saving;
            return Result;
        }

        [HttpPost("sub/{id}/one")]
        public DTO.Messages.Wrapper SaveOneSub([FromBody]DTO.Messages.Category Cat, string id = "")
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });
            var Category = CatRepo.GetOne(id);
            if (Cat == null || string.IsNullOrWhiteSpace(Cat.Name))
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            if (Category == null)
            {
                Result.Messages.Add("Parameter", "ID can't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
                return Result;
            }
            var Saving = new DTO.Databases.SubCategory
            {
                Name = Cat.Name,
                Owner = Token.sub,
                ParentId = Category.Id
            };

            Result.Messages.Add("Update", SubRepo.Save(Saving) ? "Ok" : "Failed");
            Result.Data = Saving;
            return Result;
        }
    }
}