using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Models
{
    public class Property
    {
        public int PropertyId { get; set; }

        public string PropertyType { get; set; }

        public string Locality { get; set; }

        public int Budget { get; set; }
    }
}
