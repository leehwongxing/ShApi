using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using System.Collections.Generic;
using System.IO;

namespace API.Controllers
{
    [Route("api/resource")]
    public class ResourceController : BaseController
    {
        private IOptionsSnapshot<Configs.Resource> ResMons { get; set; }

        private Repositories.Mongo.Resource ResRepo { get; set; }

        public ResourceController(IOptionsSnapshot<Configs.Mongo> options, IOptionsSnapshot<Configs.JWT> secret, IOptionsSnapshot<Configs.Resource> resMons, IHttpContextAccessor httpContextAccessor) : base(options, secret, httpContextAccessor)
        {
            ResMons = resMons;
            ResRepo = new Repositories.Mongo.Resource(MongoClient);
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
            var Result = AuthorizeResponse();
            Result.Data = id;

            var Resource = ResRepo.GetOne(id);
            if (Resource == null)
            {
                Result.Messages.Add("Resource", "not found");
            }

            if (Result.Messages.Count > 0)
            {
                Result.Code = 400;
                Result.Status = "Bad Request";
            }
            else
            {
                var Filter = new BsonDocument("_id", id);
                ResRepo.Collection.DeleteOne(Filter);
                Result.Messages.Add("File Deletion", ResMons.Value.Delete(id) ? "OK" : "Fail");
            }
            return Result;
        }

        [HttpPost("upload")]
        public DTO.Messages.Wrapper UploadFiles(List<IFormFile> Uploading)
        {
            var Result = AuthorizeResponse();

            if (Uploading == null || Uploading.Count == 0)
            {
                Result.Messages.Add("Uploading", "file(s) not found");
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

                Files.Add(UploadedResource);
                using (var FileStream = ResMons.Value.CreateWriteStream(UploadedResource.Id))
                {
                    Uploaded.CopyTo(FileStream);
                    FileStream.Flush();
                }
            }
            Result.Data = Files;
            ResRepo.Collection.InsertMany(Files);
            return Result;
        }
    }
}