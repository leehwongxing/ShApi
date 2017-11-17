using System;
using System.Collections.Generic;
using System.Text;

namespace DTO.Databases
{
    public class Permission : Owned
    {
        public string Id { get { return GetId(); } }

        public bool Owned { get; set; }

        public string Granted { get; set; }

        public string Description { get; set; }

        public Permission() : base()
        {
            Group = "PERMISSION";
            Owned = false;
            Granted = "NONE";
            Description = "EMPTY";
        }

        private string GetId()
        {
            return string.Join("__",
                Group,
                (Owned ? "TRUE" : "FALSE"),
                (Granted.Replace(" ", "").Replace("-", ""))
                ).ToUpperInvariant();
        }
    }
}