using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Models
{
    public class AssignExecutive
    {
        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int ExecutiveId { get; set; }
    }
}
