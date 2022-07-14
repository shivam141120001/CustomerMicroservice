using CustomerMicroservice.Models;
using CustomerMicroservice.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using CustomerMicroservice.Models.Auth;
using CustomerMicroservice.Models.Response;
using CustomerMicroservice.Models.ErrorHandler;
using CustomerMicroservice.Middleware;
using Newtonsoft.Json;

namespace CustomerMicroservice.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class CustomerController : Controller
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IAuthMiddleware _authMiddleware;
        private readonly log4net.ILog _log = log4net.LogManager.GetLogger(typeof(CustomerController));

        public CustomerController(ICustomerRepository customerRepository, IAuthMiddleware authMiddleware)
        {
            _customerRepository = customerRepository;
            _authMiddleware = authMiddleware;
        }

        private bool TryExtractToken(out TokenString tokenString)
        {
            try
            {
                //_log.Info("TryExtractToken: Process Initiated");

                tokenString = null;
                
                if (!HttpContext.Request.Headers.ContainsKey("Authorization"))
                {
                    //_log.Info("TryExtractToken: Process Terminated With Message: Request headers does not contain Authorization Token.");
                    return false;
                }
                tokenString = new TokenString
                {
                    Token = ((string)HttpContext.Request.Headers.First(x => x.Key == "Authorization").Value).Split().Last()
                };

                //_log.Info("TryExtractToken: Process Terminated Successfully.");
                return true;
            }
            catch (Exception exception)
            {
                //_log.Error("TryExtractToken: Process Terminated With Exception -", exception);
                throw;
            }
        }

        private bool TryHandleRequestAuth(string methodName, out AuthUser authUser)
        {
            try
            {
                //_log.Info("TryHandleRequestAuth: Process Initiated");
                
                authUser = null;
                
                if (!TryExtractToken(out TokenString tokenString))
                {
                    //_log.Info("TryHandleRequestAuth: Process Terminated With Message: Token can not be extracted.");
                    return false;
                }
                
                if (!_authMiddleware.ValidateToken(tokenString, out authUser))
                {
                    //_log.Info("TryHandleRequestAuth: Process Terminated With Message: Invalid Token.");
                    return false;
                }

                if (!_authMiddleware.IsRoleAuthorized(methodName, authUser.Role))
                {
                    //_log.Info("TryHandleRequestAuth: Process Terminated With Message: Unauthorized Role.");
                    return false;
                }

                //_log.Info("TryHandleRequestAuth: Process Terminated Successfully.");
                return true;
            }
            catch (Exception exception)
            {
                //_log.Error("TryHandleRequestAuth: Process Terminated With Exception -", exception);
                throw;
            }
        }

        
        //POST: https://localhost:44366/api/Customer/login
        [HttpPost]
        [Route("login")]
        public IActionResult Login([FromBody] User user)
        {
            try
            {
                _log.Info("Login: Process Initiated");
                
                if (!Role.Exists(user.Role))
                {
                    _log.Info("Login: Process Terminated With Message: Invalid Role.");
                    return Unauthorized(new UnauthorizedResponseObject(StatusCodes.Status401Unauthorized, new List<Error>() {
                        new Error(nameof(user.Role), "Invalid Role")
                    }));
                }
                
                if (!_customerRepository.UserExists(user, out AuthUser authUser))
                {
                    _log.Info("Login: Process Terminated With Message: Invalid User Credentials.");
                    return Unauthorized(new UnauthorizedResponseObject(StatusCodes.Status401Unauthorized, new List<Error>() {
                        new Error(nameof(user.Username) + " or " + nameof(user.Password), "Invalid User Credentials")
                    }));
                }
                
                _log.Info("Login: Process Terminated Successfully.");
                return Ok(_customerRepository.GetAuthToken(user));

            }
            catch (Exception exception)
            {
                _log.Error("Login: Process Terminated With Exception -", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        
        //POST : https://localhost:44366/api/Customer/validate
        [HttpPost]
        [Route("validate")]
        public IActionResult Validate(User user)
        {
            try
            {
                _log.Info("Validate: Process Initiated");
                
                if (!_customerRepository.UserExists(user, out AuthUser authUser))
                {
                    _log.Info("Validate: Process Terminated With Message: Invalid User Credentials.");
                    return Unauthorized();
                }
                
                _log.Info("Validate: Process Terminated Successfully.");
                return Ok(authUser);

            }
            catch (Exception exception)
            {
                _log.Error("Validate: Process Terminated With Exception -", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        
        // Post: https://localhost:44366/api/Customer/createCustomer
        [HttpPost]
        [Route("createCustomer")]
        public IActionResult CreateCustomer([FromBody] Customer customer)
        {
            try
            {
                _log.Info("CreateCustomer: Process Initiated");
                
                if (!_customerRepository.AddCustomer(customer))
                {
                    _log.Info("CreateCustomer: Process Terminated With Message: Customer already exists.");
                    return Conflict();
                }

                _log.Info("CreateCustomer: Process Terminated Successfully.");
                return Ok();

            }
            catch (Exception exception)
            {
                _log.Error("CreateCustomer: Process Terminated With Exception -", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
           
        }

        
        // Get: https://localhost:44366/api/Customer/getCustomerDetails?customerId=102
        [HttpGet]
        [Route("getCustomerDetails")]
        public IActionResult GetCustomerDetails([FromQuery] int customerId)
        {
            try
            {
                _log.Info("GetCustomerDetails: Process Initiated");

                if (!TryHandleRequestAuth(ControllerMethods.GET_CUSTOMER_DETAILS, out AuthUser authUser))
                {
                    _log.Info("GetCustomerDetails: Process Terminated With Message: Unauthorized Access.");
                    return Unauthorized();
                }

                var customer = _customerRepository.GetCustomerById(customerId);

                if (customer == null)
                {
                    _log.Info("GetCustomerDetails: Process Terminated With Message: Customer does not exist.");
                    return NotFound();
                }
                
                _log.Info("GetCustomerDetails: Process Terminated Successfully.");
                return Ok(customer);

            }
            catch (Exception exception)
            {
                _log.Error("GetCustomerDetails: Process Terminated With Exception -", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);

            }



        }

        
        // Get: https://localhost:44366/api/Customer/displayAllCustomers
        [HttpGet]
        [Route("displayAllCustomers")]
        public IActionResult GetCustomers()
        {
            try
            {
                _log.Info("GetCustomers: Process Initiated");
                
                if (!TryHandleRequestAuth(ControllerMethods.GET_CUSTOMERS, out AuthUser authUser))
                {
                    _log.Info("GetCustomers: Process Terminated With Message: Unauthorized Access.");
                    return Unauthorized();
                }
                
                _log.Info("GetCustomers: Process Terminated Successfully.");
                return Ok(_customerRepository.GetCustomers());

            }
            catch (Exception exception)
            {
                _log.Error("GetCustomers: Process Terminated With Exception -", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            
        }

        
        // Get: https://localhost:44366/api/Customer/getProperties
        [HttpGet]
        [Route("getProperties")]
        public IActionResult GetProperties()
        {
            try
            {
                _log.Info("GetProperties: Process Initiated");
                
                if (!TryHandleRequestAuth(ControllerMethods.GET_PROPERTIES, out AuthUser authUser))
                {
                    _log.Info("GetProperties: Process Terminated With Message: Unauthorized Access.");
                    return Unauthorized();
                }
                TryExtractToken(out TokenString token);

                if (!_customerRepository.GetProperties(token, out IEnumerable<Property> properties))
                {
                    _log.Info("GetProperties: Process Terminated With Message: Properties not fetched.");
                    return NoContent();
                }

                _log.Info("GetProperties: Process Terminated Successfully.");
                return Ok(properties);

            }
            catch (Exception exception)
            {
                _log.Error("GetProperties: Process Terminated With Exception -", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        
        // Get: https://localhost:44366/api/Customer/assignExecutive
        [HttpPut]
        [Route("assignExecutive")]
        public IActionResult AssignExecutive([FromBody] AssignExecutive newExecutive )
        {
            try
            {
                _log.Info("AssignExecutive: Process Initiated");
                
                if (!TryHandleRequestAuth(ControllerMethods.ASSIGN_EXECUTIVE, out AuthUser authUser))
                {
                    _log.Info("AssignExecutive: Process Terminated With Message: Unauthorized Access.");
                    return Unauthorized();
                }
                TryExtractToken(out TokenString token);

                if (!_customerRepository.AssignExecutive(newExecutive, token))
                {
                    _log.Info("AssignExecutive: Process Terminated With Message: Executive can not be assigned to Customer");
                    return NoContent();
                }
                
                _log.Info("AssignExecutive: Process Terminated Successfully.");
                return Ok();

            }
            catch (Exception exception)
            {
                _log.Error("AssignExecutive: Process Terminated With Exception -", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        
        // Get: https://localhost:44366/api/Customer/getAllCustomersByExecutive
        [HttpGet]
        [Route("getAllCustomersByExecutive")]
        public IActionResult GetAllCustomersByExecutive([FromQuery] int executiveId)
        {
            try
            {
                _log.Info("GetAllCustomersByExecutive: Process Initiated");

                if (!TryHandleRequestAuth(ControllerMethods.GET_CUSTOMERS_BY_EXECUTIVE, out AuthUser authUser))
                {
                    _log.Info("GetAllCustomersByExecutive: Process Terminated With Message: Unauthorized Access.");
                    return Unauthorized();
                }
                
                _log.Info("GetAllCustomersByExecutive: Process Terminated Successfully.");
                return Ok(_customerRepository.GetCustomersByExecutive(executiveId));

            }
            catch (Exception exception)
            {
                _log.Error("GetAllCustomersByExecutive: Process Terminated With Exception -", exception);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
