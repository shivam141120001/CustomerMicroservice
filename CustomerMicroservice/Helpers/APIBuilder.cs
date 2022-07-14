using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Helpers
{
    public static class APIBuilder
    {
        public static class AuthenticationMicroservice
        {
            private const string BASE_URL = "https://localhost:44322/api/";
        }

        public static class ManagerMicroservice
        {
            private const string BASE_URL = "https://localhost:44348/api/";
        }

        public static class PropertyMicroservice
        {
            private const string BASE_URL = "https://localhost:44328/api/";
        }
    }
}
