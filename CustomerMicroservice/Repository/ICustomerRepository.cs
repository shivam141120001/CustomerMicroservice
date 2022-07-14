using CustomerMicroservice.Models;
using CustomerMicroservice.Models.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Repository
{
    public interface ICustomerRepository
    {
        public bool UserExists(User user, out AuthUser authUser);
        public AuthToken GetAuthToken(User user);
        public bool AddCustomer(Customer customer);
        public List<Customer> GetCustomers();
        public Customer GetCustomerById(int id);
        public List<Customer> GetCustomersByExecutive(int executiveId);
        public bool AssignExecutive(AssignExecutive assignExecutive, TokenString token);
        public bool GetProperties(TokenString token, out IEnumerable<Property> properties);
    }
}
