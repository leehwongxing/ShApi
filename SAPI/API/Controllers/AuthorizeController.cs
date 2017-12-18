using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System;
using System.Linq;

namespace API.Controllers
{
    [Route("api/auth")]
    public class AuthorizeController : BaseController
    {
        private Repositories.Mongo.User UserRepo { get; set; }

        public AuthorizeController(IOptionsSnapshot<Configs.Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IHttpContextAccessor httpContextAccessor) : base(options, secret, httpContextAccessor)
        {
            UserRepo = new Repositories.Mongo.User(MongoClient);
        }

        [HttpGet("signout")]
        public DTO.Messages.Wrapper SignOut()
        {
            var Result = AuthorizeResponse();
            Result.Data = Authorization;

            if (Result.Messages.Count == 0)
            {
                var Filter = new BsonDocument("_id", Token.jti);
                TokenRepo.Collection.DeleteOne(Filter);
                Result.Messages.Add("Session", "signed out");
            }

            return Result;
        }

        [HttpGet("verify")]
        [HttpPost("verify")]
        public DTO.Messages.Wrapper VerifyToken()
        {
            var Result = AuthorizeResponse();
            Result.Data = Authorization;
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

            DTO.Databases.User User = null;

            if (Result.Messages.Count == 0)
            {
                var Count = UserRepo.QueryableCollection.Where(x => x.Email == Login.Email).Count();
                if (Count == 0)
                {
                    Result.Messages.Add("Account", "Email is not registered");
                }

                User = UserRepo.QueryableCollection.Where(x => x.Email == Login.Email).First();
                if (!Configs.Hashing.Compare(Login.Password, User.Id, User.Password))
                {
                    Result.Messages.Add("Account", "Password mismatched");
                }
            }

            if (Result.Messages.Count > 0)
            {
                Result.Status = "Bad Request";
                Result.Code = 400;
            }
            else
            {
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