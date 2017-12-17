using System.Collections.Generic;
using System.Linq;

namespace API.Configs
{
    public class Permissions
    {
        public Dictionary<string, bool> List { get; private set; }

        public Permissions(Dictionary<string, bool> Default = null)
        {
            if (Default == null)
            {
                List = new Dictionary<string, bool>();
            }
            else
            {
                List = Default;
            }
        }

        public IEnumerable<string> GetPermissions(string Role = "Default")
        {
            switch (Role)
            {
                case "Default":
                    return GetDefault();

                case "Administrator":
                    return GetAll();

                default:
                    return GetPermissionsForRole(Role);
            }
        }

        private IEnumerable<string> GetDefault()
        {
            return List.Where(x => x.Value == true).Select(x => x.Key);
        }

        private IEnumerable<string> GetAll()
        {
            return List.Select(x => x.Key);
        }

        private IEnumerable<string> GetPermissionsForRole(string Role = "")
        {
            if (string.IsNullOrWhiteSpace(Role))
            {
                throw new System.Exception("Role mustn't be empty");
            }
            return null;
        }
    }
}