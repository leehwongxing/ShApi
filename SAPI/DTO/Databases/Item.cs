using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Databases
{
    public class Item : Owned
    {
        public string Id { get; set; }

        public string Name { get; set; }
    }
}