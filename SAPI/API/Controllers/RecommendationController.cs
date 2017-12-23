using API.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("api/recommendation")]
    public class RecommendationController : BaseController
    {
        private Repositories.Mongo.Recommendation Re2po { get; set; }

        private Repositories.Mongo.Product ProductRepo { get; set; }

        public RecommendationController(IOptionsSnapshot<Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor) : base(options, secret, httpContextAccessor)
        {
            Re2po = new Repositories.Mongo.Recommendation(MongoClient);
            ProductRepo = new Repositories.Mongo.Product(MongoClient);
        }

        [HttpGet("all")]
        public DTO.Messages.Wrapper GetAll()
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            Result.Data = Re2po.QueryableCollection.ToList();
            return Result;
        }

        [HttpPost("create")]
        public DTO.Messages.Wrapper CreateOne([FromBody] DTO.Messages.Recommendation Rec)
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });
            if (Rec == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Rec.Reason) || Rec.Reason.Length < 5)
                {
                    Result.Messages.Add("Reason", "can't be empty or shorter than 5 characters");
                }
                if (Rec.List.Count < 2)
                {
                    Result.Messages.Add("List", "please add at least 2 in recommedations");
                }
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var NewRec = new DTO.Databases.Recommendation
            {
                Reason = Rec.Reason,
                List = Rec.List,
                Owner = Token.sub
            };

            var Created = Re2po.Save(NewRec);
            Result.Data = NewRec;
            Result.Messages.Add("Created", Created ? "Ok" : "Failed");
            return Result;
        }

        [HttpPost("update")]
        public DTO.Messages.Wrapper SaveOne([FromBody] DTO.Databases.Recommendation Rec)
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });
            if (Rec == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Rec.Id))
                {
                    Result.Messages.Add("Id", "can't be empty");
                }
                if (string.IsNullOrWhiteSpace(Rec.Reason) || Rec.Reason.Length < 5)
                {
                    Result.Messages.Add("Reason", "can't be empty or shorter than 5 characters");
                }
                if (Rec.List.Count < 2)
                {
                    Result.Messages.Add("List", "please add at least 2 in recommedations");
                }
            }
            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var Saved = Re2po.Save(Rec);
            Result.Data = Rec;
            Result.Messages.Add("Update", Saved ? "Ok" : "Failed");
            return Result;
        }

        [HttpPost("delete/{id}")]
        [HttpGet("delete/{id}")]
        public DTO.Messages.Wrapper DeleteOne(string id = "")
        {
            var Result = AuthorizeResponse(new HashSet<string> { "Administrator" });
            Result.Data = id;

            if (string.IsNullOrWhiteSpace(id))
            {
                Result.Messages.Add("Id", "can't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var Deleted = Re2po.Delete(new DTO.Databases.Recommendation { Id = id });
            Result.Messages.Add("Delete", Deleted ? "Ok" : "Failed");
            return Result;
        }

        [HttpGet("of/{id}")]
        [HttpPost("of/{id}")]
        public DTO.Messages.Wrapper GetOf(string id)
        {
            var Result = new DTO.Messages.Wrapper();

            if (string.IsNullOrWhiteSpace(id))
            {
                Result.Messages.Add("Id", "can't be empty");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var List = Re2po.QueryableCollection
                            .Where(x => x.List.Contains(id))
                            .SelectMany(x => x.List)
                            .Distinct()
                            .ToList();
            List.Remove(id);
            if (List.Count == 0)
            {
                Result.Code = 404;
                Result.Status = "Not Found";
                Result.Data = null;

                return Result;
            }

            List.Shuffle();
            var Conditions = List.Take((List.Count < 5) ? List.Count : 5);

            var Return = ProductRepo.QueryableCollection
                            .Where(x => Conditions.Contains(x.Id))
                            .Select(x => new DTO.Projection.Recommendation
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Images = x.Images,
                                Price = x.Price
                            });

            Result.Data = Return;
            return Result;
        }
    }
}