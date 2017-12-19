using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Databases;

namespace API.Repositories.Redis
{
    public class Product : Base<DTO.Databases.Product>
    {
        public Product(Databases.Redis client) : base(client, "Product")
        {
        }
    }
}