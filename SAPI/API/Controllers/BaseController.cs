using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Collections.Generic;
using System.Linq;

namespace API.Controllers
{
    public class BaseController : Controller
    {
        protected IOptionsSnapshot<Configs.Mongo> Options { get; set; }

        protected IOptionsSnapshot<Configs.JWT> Sekrit { get; set; }

        protected IHttpContextAccessor Context { get; set; }

        protected string Authorization { get; set; }

        protected DTO.Tokens.JWT Token { get; set; }

        protected Databases.Mongo MongoClient { get; set; }

        protected Repositories.Mongo.Token TokenRepo { get; set; }

        public BaseController(IOptionsSnapshot<Configs.Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor)
        {
            Options = options;
            Sekrit = secret;
            Context = httpContextAccessor;
            Token = null;

            MongoClient = new Databases.Mongo(Options);
            TokenRepo = new Repositories.Mongo.Token(MongoClient);

            var Headers = Context.HttpContext.Request.Headers;
            var Authorized = Headers.Where(x => x.Key == "Authorization").Select(x => x.Value).FirstOrDefault();
            Authorization = (string.IsNullOrEmpty(Authorized)) ? "" : ((string)Authorized).Substring(7);
        }

        protected DTO.Messages.Wrapper AuthorizeResponse(HashSet<string> Roles = null)
        {
            var Result = new DTO.Messages.Wrapper();

            var Verification = Sekrit.Value.Verify(Authorization);

            switch (Verification)
            {
                case 1:
                    break;

                case 0:
                    Result.Messages.Add("Authorization", "Token's header not found");
                    break;

                case -1:
                    Result.Messages.Add("Expiration", "Token expirated");
                    break;

                case -2:
                    Result.Messages.Add("Signature", "Token can't be verified");
                    break;

                default:
                    break;
            }
            if (Verification == 1)
            {
                Token = Sekrit.Value.Decode(Authorization);
                var Session = TokenRepo.GetOne(Token.jti);

                if (Session == null)
                {
                    Result.Messages.Add("Session", "not found");
                }
            }

            if (Roles != null && Roles.Count > 0)
            {
                var Origin = new HashSet<string>
                {
                    Token.aud
                };
                Origin.IntersectWith(Roles);

                if (Origin.Count == 0)
                {
                    Result.Messages.Add("Role", "failed to meet conditions");
                }
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
            }
            return Result;
        }
    }
}