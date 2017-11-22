using System.Collections.Generic;
using System.Linq;

namespace API.Configs
{
    public class Permissions
    {
        public Dictionary<string, bool> PermList { get; private set; }

        public Permissions(Dictionary<string, bool> Default = null)
        {
            if (Default == null)
            {
                PermList = new Dictionary<string, bool>();
            }
            else
            {
                PermList = Default;
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
            return PermList.Where(x => x.Value == true).Select(x => x.Key);
        }

        private IEnumerable<string> GetAll()
        {
            return PermList.Select(x => x.Key);
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