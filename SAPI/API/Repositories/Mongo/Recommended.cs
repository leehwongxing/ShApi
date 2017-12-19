using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Databases;
using DTO.Databases;

namespace API.Repositories.Mongo
{
    public class Recommended : Base<DTO.Databases.Recommended>
    {
        public Recommended(Databases.Mongo client) : base(client, "Recommended")
        {
        }

        public override bool Delete(DTO.Databases.Recommended Document)
        {
            throw new NotImplementedException();
        }

        public override DTO.Databases.Recommended GetOne(string Id)
        {
            throw new NotImplementedException();
        }

        public override bool Save(DTO.Databases.Recommended Document)
        {
            throw new NotImplementedException();
        }
    }
}