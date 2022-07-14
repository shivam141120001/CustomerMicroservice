using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Models.Response
{
    public class ResponseObject
    {
        public string Title { get; set; }
        public int Status { get; set; }

        public ResponseObject() { }
        public ResponseObject(int status)
        {
            this.Status = status;
            this.Title = ReasonPhrases.GetReasonPhrase(status);
        }
    }
}
