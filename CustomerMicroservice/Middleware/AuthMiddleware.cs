using CustomerMicroservice.Models;
using CustomerMicroservice.Models.Auth;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CustomerMicroservice.Middleware
{
    public class AuthMiddleware : IAuthMiddleware
    {
        private readonly IDictionary<string, List<string>> _AuthorizedRoles = new Dictionary<string, List<string>>()
        {
            { ControllerMethods.LOGIN, new List<string>() },
            { ControllerMethods.VALIDATE, new List<string>() },
            { ControllerMethods.CREATE_CUSTOMER, new List<string>() },
            { ControllerMethods.GET_CUSTOMER_DETAILS, new List<string>() { Role.Manager } },
            { ControllerMethods.GET_CUSTOMERS, new List<string>() { Role.Manager } },
            { ControllerMethods.GET_PROPERTIES, new List<string>() { Role.Customer } },
            { ControllerMethods.ASSIGN_EXECUTIVE, new List<string>() { Role.Manager } },
            { ControllerMethods.GET_CUSTOMERS_BY_EXECUTIVE, new List<string>() { Role.Executive } },
        };

        public bool ValidateToken(TokenString token, out AuthUser authUser)
        {
            try
            {
                authUser = null;
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClientHandler clientHandler = new HttpClientHandler();
                HttpContent content = new StringContent(JsonConvert.SerializeObject(token), Encoding.UTF8, "application/json");
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    response = client.PostAsync("https://localhost:44322/api/auth/jwt/validate", content).Result;
                    if ((int)response.StatusCode != 200) return false;
                    authUser = JsonConvert.DeserializeObject<AuthUser>(response.Content.ReadAsStringAsync().Result);
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsRoleAuthorized(string methodName, string role)
        {
            try
            {
                if (!Role.Exists(role) || !ControllerMethods.Exists(methodName) || !_AuthorizedRoles.ContainsKey(methodName)) return false;
                return _AuthorizedRoles.First(x => x.Key == methodName).Value.Contains(role);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
