using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Linq;

namespace API.Controllers
{
    [Route("api/auth")]
    public class AuthorizeController : Controller
    {
        private IOptionsSnapshot<Configs.Mongo> Options { get; set; }

        private IOptionsSnapshot<Configs.JWT> Sekrit { get; set; }

        private IHttpContextAccessor Context { get; set; }

        private string Authorization { get; set; }

        private Databases.Mongo MongoClient { get; set; }

        private Repositories.Mongo.User UserRepo { get; set; }

        private Repositories.Mongo.Token TokenRepo { get; set; }

        public AuthorizeController(IOptionsSnapshot<Configs.Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor)
        {
            Options = options;
            Sekrit = secret;
            Context = httpContextAccessor;

            MongoClient = new Databases.Mongo(Options);
            UserRepo = new Repositories.Mongo.User(MongoClient);
            TokenRepo = new Repositories.Mongo.Token(MongoClient);

            var Headers = Context.HttpContext.Request.Headers;
            var Authorized = Headers.Where(x => x.Key == "Authorization").Select(x => x.Value).FirstOrDefault();
            Authorization = (string.IsNullOrEmpty(Authorized)) ? "" : ((string)Authorized).Substring(7);
        }

        [HttpGet("signout")]
        public DTO.Messages.Wrapper SignOut()
        {
            var Result = new DTO.Messages.Wrapper
            {
                Data = Authorization
            };

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

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
            }
            else
            {
                var Token = Sekrit.Value.Decode(Authorization);
                var Count = TokenRepo.QueryableCollection.Where(x => x.TokenId == Token.jti).Count();
                if (Count == 0)
                {
                    Result.Messages.Add("Session", "not found");
                    Result.Status = "Bad Request";
                    Result.Code = 400;
                }
                else
                {
                    var Filter = new BsonDocument("_id", Token.jti);
                    TokenRepo.Collection.DeleteOne(Filter);
                    Result.Messages.Add("Session", "signed out");
                }
            }

            return Result;
        }

        [HttpGet("verify")]
        public DTO.Messages.Wrapper VerifyToken()
        {
            var Result = new DTO.Messages.Wrapper
            {
                Data = Authorization
            };

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

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
            }
            else
            {
                var Token = Sekrit.Value.Decode(Authorization);
                var Count = TokenRepo.QueryableCollection.Where(x => x.TokenId == Token.jti).Count();
                if (Count == 0)
                {
                    Result.Messages.Add("Session", "not found");
                    Result.Status = "Bad Request";
                    Result.Code = 400;
                }
                else
                {
                    Result.Messages.Add("Verification", "OK");
                }
            }
            return Result;
        }

        public DTO.Tokens.SignedIn OnSignedIn(DTO.Databases.User User)
        {
            // genrate new token here
            var SignInData = new DTO.Tokens.SignedIn
            {
                UserId = User.Id,
                Email = User.Email,
                FullName = User.Fullname
            };
            var Token = new DTO.Tokens.JWT(Sekrit.Value.TTL)
            {
                sub = User.Id
            };

            SignInData.Token = Sekrit.Value.Encode(Token);

            var Session = new DTO.Databases.Token
            {
                TokenId = Token.jti,
                UserId = User.Id,
                Owner = User.Id
            };

            TokenRepo.Save(Session);
            return SignInData;
        }

        [HttpPost("signin")]
        public DTO.Messages.Wrapper SignIn([FromBody] DTO.Messages.SigningInUser Login)
        {
            var Result = new DTO.Messages.Wrapper();

            if (Login == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Login.Email) || !DTO.Generator.IsEmail(Login.Email))
                {
                    Result.Messages.Add("Email", "must be a valid Email address");
                }
                if (string.IsNullOrWhiteSpace(Login.Password) || Login.Password.Length < 12)
                {
                    Result.Messages.Add("Password", "must be at least 12 characters");
                }
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
                return Result;
            }
            else
            {
                var Count = UserRepo.QueryableCollection.Where(x => x.Email == Login.Email).Count();
                if (Count == 0)
                {
                    Result.Messages.Add("Account", "Email is not registered");
                }

                var User = UserRepo.QueryableCollection.Where(x => x.Email == Login.Email).First();
                if (!Configs.Hashing.Compare(Login.Password, User.Id, User.Password))
                {
                    Result.Messages.Add("Account", "Password mismatched");
                }

                Result.Data = OnSignedIn(User);
            }

            return Result;
        }

        [HttpPost("register")]
        public DTO.Messages.Wrapper Register([FromBody] DTO.Messages.RegisteringUser Registry)
        {
            var Result = new DTO.Messages.Wrapper();

            if (Registry == null)
            {
                Result.Messages.Add("PostBody", "can't be empty");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(Registry.Email))
                {
                    Result.Messages.Add("Email", "can't be empty");
                }
                else if (!DTO.Generator.IsEmail(Registry.Email))
                {
                    Result.Messages.Add("Email", "must be a valid Email");
                }
                if (string.IsNullOrWhiteSpace(Registry.Password) || Registry.Password.Length < 12)
                {
                    Result.Messages.Add("Password", "can't be less than 12 characters");
                }
                if (string.IsNullOrWhiteSpace(Registry.Fullname))
                {
                    Result.Messages.Add("FullName", "can't be empty");
                }
                if (Registry.Gender < 0 || Registry.Gender > 1)
                {
                    Result.Messages.Add("Gender", "can't be lower than 0 or higher than 1");
                }
                try
                {
                    Convert.ToDateTime(Registry.Birthday, new System.Globalization.CultureInfo("en-US"));
                }
                catch (FormatException)
                {
                    Result.Messages.Add("Birthday", "must be formated as 'M/d/YYYY'");
                }

                var Count = UserRepo.QueryableCollection.Where(x => x.Email == Registry.Email).Count();
                if (Count > 0)
                {
                    Result.Messages.Add("Account", "Email is registered");
                }
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;

                return Result;
            }
            // do registering user here

            var NewUser = new DTO.Databases.User
            {
                Email = Registry.Email,
                Fullname = Registry.Fullname,
                Birthday = Registry.Birthday,
                Gender = Registry.Gender
            };

            NewUser.Password = Configs.Hashing.Hash(Registry.Password, NewUser.Id);
            UserRepo.Save(NewUser);

            Result.Data = OnSignedIn(NewUser);
            return Result;
        }
    }
}