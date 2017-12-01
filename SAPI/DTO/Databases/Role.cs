using System.Collections.Generic;

namespace DTO.Databases
{
    public class Role : Owned
    {
        public string Id { get { return GetId(); } }

        public string Granted { get; set; }

        public HashSet<string> Permissions { get; set; }

        public Role() : base()
        {
            Granted = "";
            Permissions = new HashSet<string>();
        }

        public string GetId()
        {
            return string.Join("__",
                Group,
                Generator.StripAccents(Granted).Replace("-", "").Replace("_", "")
                ).ToUpperInvariant();
        }
    }
}