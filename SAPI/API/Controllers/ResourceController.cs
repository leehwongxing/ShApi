﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace API.Controllers
{
    [Route("api/resource")]
    public class ResourceController : Controller
    {
        private IOptionsSnapshot<Configs.Mongo> Options { get; set; }

        private IOptionsSnapshot<Configs.JWT> Sekrit { get; set; }

        private IOptionsSnapshot<Configs.Resource> ResMons { get; set; }

        private IHttpContextAccessor Context { get; set; }

        private string Authorization { get; set; }

        private Databases.Mongo MongoClient { get; set; }

        private Repositories.Mongo.Resource ResRepo { get; set; }

        private Repositories.Mongo.Token TokenRepo { get; set; }

        public ResourceController(IOptionsSnapshot<Configs.Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IOptionsSnapshot<Configs.Resource> resMons, IHttpContextAccessor httpContextAccessor)
        {
            Options = options;
            Sekrit = secret;
            Context = httpContextAccessor;
            ResMons = resMons;

            MongoClient = new Databases.Mongo(Options);
            ResRepo = new Repositories.Mongo.Resource(MongoClient);
            TokenRepo = new Repositories.Mongo.Token(MongoClient);

            var Headers = Context.HttpContext.Request.Headers;
            var Authorized = Headers.Where(x => x.Key == "Authorization").Select(x => x.Value).FirstOrDefault();
            Authorization = (string.IsNullOrEmpty(Authorized)) ? "" : ((string)Authorized).Substring(7);
        }

        [HttpGet("download/{id}")]
        public IActionResult DownloadFile(string id, [FromQuery] int dl = 0)
        {
            var Resource = ResRepo.GetOne(id);
            if (!ResMons.Value.Exist(id) || Resource == null)
            {
                return NotFound("File not found");
            }

            var Stream = ResMons.Value.CreateReadStream(id);
            if (dl != 0)
            {
                return File(Stream, Resource.MiMe, Resource.FileName);
            }
            else
            {
                return File(Stream, Resource.MiMe);
            }
        }

        [HttpDelete("delete/{id}")]
        [HttpGet("delete/{id}")]
        [HttpPost("delete/{id}")]
        public DTO.Messages.Wrapper DeleteFile(string id)
        {
            var Result = new DTO.Messages.Wrapper
            {
                Data = id
            };
            DTO.Tokens.JWT Token = null;

            var Signed = Sekrit.Value.Verify(Authorization);
            if (Signed != 1)
            {
                Result.Messages.Add("Authorization", "failed");
            }
            else
            {
                Token = Sekrit.Value.Decode(Authorization);
                var Session = TokenRepo.GetOne(Token.jti);

                if (Session == null)
                {
                    Result.Messages.Add("Session", "not found");
                }
            }

            var Resource = ResRepo.GetOne(id);
            if (Resource == null)
            {
                Result.Messages.Add("Resource", "not found");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";

                return Result;
            }

            var Filter = new BsonDocument("_id", id);
            ResRepo.Collection.DeleteOne(Filter);

            Result.Messages.Add("File Deletion", ResMons.Value.Delete(id) ? "OK" : "Fail");
            return Result;
        }

        [HttpPost("upload")]
        public DTO.Messages.Wrapper UploadFiles(List<IFormFile> Uploading)
        {
            var Result = new DTO.Messages.Wrapper();
            DTO.Tokens.JWT Token = null;

            var Signed = Sekrit.Value.Verify(Authorization);
            if (Signed != 1)
            {
                Result.Messages.Add("Authorization", "failed");
            }
            else
            {
                Token = Sekrit.Value.Decode(Authorization);
                var Session = TokenRepo.GetOne(Token.jti);

                if (Session == null)
                {
                    Result.Messages.Add("Session", "not found");
                }

                if (Uploading == null || Uploading.Count == 0)
                {
                    Result.Messages.Add("Uploading", "file(s) not found");
                }
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";

                return Result;
            }

            var Files = new HashSet<DTO.Databases.Resource>();

            foreach (var Uploaded in Uploading)
            {
                var UploadedResource = new DTO.Databases.Resource
                {
                    FileName = Uploaded.FileName,
                    MiMe = Uploaded.ContentType,
                    Length = Uploaded.Length,
                    Owner = Token.sub
                };

                using (var FileStream = ResMons.Value.CreateWriteStream(UploadedResource.Id))
                {
                    Uploaded.CopyTo(FileStream);
                    FileStream.Flush();
                }
                Files.Add(UploadedResource);
            }

            ResRepo.Collection.InsertMany(Files);
            Result.Data = Files;

            return Result;
        }
    }
}