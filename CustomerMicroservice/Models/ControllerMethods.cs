using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Models
{
    public static class ControllerMethods
    {
        public const string LOGIN = "Login";
        public const string VALIDATE = "Validate";
        public const string CREATE_CUSTOMER = "CreateCustomer";
        public const string GET_CUSTOMER_DETAILS = "GetCustomerDetails";
        public const string GET_CUSTOMERS = "GetCustomers";
        public const string GET_PROPERTIES = "GetProperties";
        public const string ASSIGN_EXECUTIVE = "AssignExecutive";
        public const string GET_CUSTOMERS_BY_EXECUTIVE = "GetAllCustomersByExecutive";

        private static readonly List<string> _Methods = new List<string>()
        {
            LOGIN, VALIDATE, CREATE_CUSTOMER, GET_CUSTOMER_DETAILS, GET_CUSTOMERS, GET_PROPERTIES, ASSIGN_EXECUTIVE, GET_CUSTOMERS_BY_EXECUTIVE
        };

        public static bool Exists(string methodName)
        {
            return _Methods.Contains(methodName);
        }
    }
}
