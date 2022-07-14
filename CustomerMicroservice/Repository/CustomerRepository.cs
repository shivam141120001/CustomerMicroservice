using CustomerMicroservice.Context;
using CustomerMicroservice.Models;
using CustomerMicroservice.Models.Auth;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace CustomerMicroservice.Repository
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly CustomerMicroserviceDbContext _customerMicroserviceDbContext;
        public CustomerRepository(CustomerMicroserviceDbContext customerMicroserviceDbContext)
        {

            _customerMicroserviceDbContext = customerMicroserviceDbContext;
        }

        public AuthToken GetAuthToken(User user)
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClientHandler clientHandler = new HttpClientHandler();
                HttpContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    response = client.PostAsync("https://localhost:44322/api/auth/jwt", content).Result;
                    if ((int)response.StatusCode != 200) throw new Exception("Didn't got the token");
                    return JsonConvert.DeserializeObject<AuthToken>(response.Content.ReadAsStringAsync().Result);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool UserExists(User user, out AuthUser authUser)
        {
            try
            {
                authUser = null;
                Customer customer = _customerMicroserviceDbContext.Customers.Where(c => c.Username == user.Username && c.Password == user.Password).FirstOrDefault();
                if (customer == null) return false;
                authUser = new AuthUser
                {
                    Id = customer.CustomerId,
                    Role = Role.Customer
                };
                return true;
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public bool AddCustomer(Customer customer)
        {
            try
            {
                if (_customerMicroserviceDbContext.Customers.Any(c => c.CustomerId == customer.CustomerId || c.Username == customer.Username || c.EmailId == customer.EmailId))
                {
                    return false;
                }
                _customerMicroserviceDbContext.Customers.Add(customer);
                _customerMicroserviceDbContext.SaveChanges();
                return true;

            }
            catch (Exception)
            {
                throw;
            }
        }

        public Customer GetCustomerById(int id)
        {
            try
            {
               return _customerMicroserviceDbContext.Customers.Find(id);
            }
            catch (Exception)
            {
                throw;
            }
        }
        
        public List<Customer> GetCustomers()
        {
            try
            {
                return _customerMicroserviceDbContext.Customers.ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool AssignExecutive(AssignExecutive assignExecutive, TokenString token)
        {
            try
            {
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44348/api/");
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                    response = client.GetAsync("Manager/getExecutiveById?executiveId="+assignExecutive.ExecutiveId).Result;
                    if ((int)response.StatusCode != 200) return false;
                }
                var found = _customerMicroserviceDbContext.Customers.Find(assignExecutive.CustomerId);
                if (found == null) return false;
                
                found.ExecutiveId = assignExecutive.ExecutiveId;
                _customerMicroserviceDbContext.SaveChanges();
                return true;
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public List<Customer> GetCustomersByExecutive(int executiveId)
        {
            try
            {
                return _customerMicroserviceDbContext.Customers.Where(c => c.ExecutiveId == executiveId).ToList();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool GetProperties(TokenString token, out IEnumerable<Property> properties)
        {
            try
            {
                properties = null;
                HttpResponseMessage response = new HttpResponseMessage();
                HttpClientHandler clientHandler = new HttpClientHandler();
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://localhost:44328/api/");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    response = client.GetAsync("Property/allProperties").Result;
                    
                    if ((int)response.StatusCode != 200) return false;
                    
                    properties = JsonConvert.DeserializeObject<IEnumerable<Property>>(response.Content.ReadAsStringAsync().Result);
                    return true;
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
