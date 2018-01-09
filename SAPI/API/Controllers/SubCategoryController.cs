using API.Configs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Linq;

namespace API.Controllers
{
    [Route("subcategory")]
    public class SubCategoryController : BaseController
    {
        private Repositories.Mongo.SubCategory SubRepo { get; set; }

        public SubCategoryController(IOptionsSnapshot<Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor) : base(options, secret, httpContextAccessor)
        {
            SubRepo = new Repositories.Mongo.SubCategory(MongoClient);
        }

        [HttpGet("all")]
        public DTO.Messages.Wrapper GetAll()
        {
            var Result = new DTO.Messages.Wrapper
            {
                Data = SubRepo.QueryableCollection.Select(Sub => new DTO.Projection.SubCategory { Name = Sub.Name, Id = Sub.Id, Parent = Sub.ParentId })
            };
            return Result;
        }
    }
}