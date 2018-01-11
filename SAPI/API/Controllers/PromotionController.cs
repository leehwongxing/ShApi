using API.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    [Route("promotion")]
    public class PromotionController : BaseController
    {
        private Repositories.Mongo.Promotion PromoRepo { get; set; }

        private Repositories.Mongo.Product ItemRepo { get; set; }

        public PromotionController(IOptionsSnapshot<Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor) : base(options, secret, httpContextAccessor)
        {
            PromoRepo = new Repositories.Mongo.Promotion(MongoClient);
            ItemRepo = new Repositories.Mongo.Product(MongoClient);
        }

        [HttpGet("all")]
        [HttpPost("all")]
        public DTO.Messages.Wrapper FilterAll([FromBody] DTO.Messages.FilterPromotion Filter = null)
        {
            var Result = AuthorizeResponse(new HashSet<string>() { "Administrator" });

            if (Filter == null)
            {
                Filter = new DTO.Messages.FilterPromotion();
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            IQueryable<DTO.Databases.Promotion> Promo = null;
            if (string.IsNullOrWhiteSpace(Filter.Name))
            {
                Promo = PromoRepo.QueryableCollection;
            }
            else
            {
                Promo = PromoRepo.Collection
                    .FindSync(Builders<DTO.Databases.Promotion>.Filter.Text(Filter.Name))
                    .ToList().AsQueryable();
            }

            Result.Data = Promo;
            return Result;
        }

        [HttpPost("save")]
        public DTO.Messages.Wrapper SaveOne([FromBody] DTO.Projection.Promotion Promo = null)
        {
            var Result = AuthorizeResponse(new HashSet<string>() { "Administrator" });

            if (Promo == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            if (string.IsNullOrWhiteSpace(Promo.Name))
            {
                Result.Messages.Add("Name", "can't be empty");
            }
            if (Promo.StartDate == null || Promo.EndDate == null || Promo.StartTime == null || Promo.EndTime == null)
            {
                Result.Messages.Add("Details", "please be more specific");
            }
            if (Promo.ShownList.Count == 0 || Promo.HiddenList.Count == 0)
            {
                Result.Messages.Add("Products", "please be add some to your promotion");
            }
            if (Promo.StartDate >= Promo.EndDate)
            {
                Result.Messages.Add("Dates", "StartDate must be smaller than EndDate");
            }
            if (Promo.StartTime >= Promo.EndTime)
            {
                Result.Messages.Add("Times", "StartTime must be smaller than EndTime");
            }
            if (Promo.DaysOfWeek.Count == 0)
            {
                Result.Messages.Add("DaysOfWeek", "please be add some DaysOfWeek this would be applied");
            }
            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            var Saving = new DTO.Databases.Promotion
            {
                Name = Promo.Name,
                Description = Promo.Description,
                StartDate = Promo.StartDate,
                EndDate = Promo.EndDate,
                StartTime = Promo.EndTime,
                EndTime = Promo.EndTime,
                DaysOfWeek = Promo.DaysOfWeek,
                HiddenList = Promo.HiddenList,
                ShownList = Promo.ShownList,
                Shown = true
            };
            Saving.ShownList.ExceptWith(Saving.HiddenList);
            PromoRepo.Save(Saving);

            var List = Saving.ShownList.Union(Saving.HiddenList);
            var Modifying = ItemRepo.QueryableCollection.Where(x => List.Contains(x.Id)).ToList();

            foreach (var Item in Modifying)
            {
                if (List.Contains(Item.Id))
                {
                    Saving.Shown = true;
                }
                else
                {
                    Saving.Shown = false;
                }
                Item.Promotions.Add(Saving.Id, Saving);
                ItemRepo.Save(Item);
            }
            return Result;
        }

        [HttpGet("delete/{id}")]
        public DTO.Messages.Wrapper DeleteOne()
        {
            var Result = AuthorizeResponse(new HashSet<string>() { "Administrator" });

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
                return Result;
            }

            return Result;
        }
    }
}