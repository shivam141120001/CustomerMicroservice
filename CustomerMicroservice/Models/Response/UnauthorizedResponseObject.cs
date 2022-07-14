using CustomerMicroservice.Models.ErrorHandler;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Models.Response
{
    public class UnauthorizedResponseObject : ResponseObject
    {
        public List<Error> Errors { get; set; }

        public UnauthorizedResponseObject() { }

        public UnauthorizedResponseObject(int status) : base(status) { }

        public UnauthorizedResponseObject(int status, List<Error> errors) : base(status)
        {
            this.Errors = errors;
        }
    }
}
