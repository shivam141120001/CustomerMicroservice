using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Models
{
    public static class Role
    {
        public const string Manager = "Manager";
        public const string Executive = "Executive";
        public const string Customer = "Customer";

        private static readonly List<string> _Roles = new List<string>()
        {
            Manager, Executive, Customer
        };

        public static bool Exists(string role)
        {
            return _Roles.Contains(role);
        }
    }
}
