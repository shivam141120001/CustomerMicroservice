using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Models.ErrorHandler
{
    public class Error
    {
        public string Name { get; set; }
        public string Description { get; set; }
        
        public Error() { }
        
        public Error(string name, string description)
        {
            this.Name = name;
            this.Description = description;
        }
    }
}
