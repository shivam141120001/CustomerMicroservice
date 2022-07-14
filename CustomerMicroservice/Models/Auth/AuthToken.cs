using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Models.Auth
{
    public class AuthToken
    {
        public AuthUser User { get; set; }
        public string Token { get; set; }
    }
}
