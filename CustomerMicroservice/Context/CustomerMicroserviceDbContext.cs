using CustomerMicroservice.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerMicroservice.Context
{
    public class CustomerMicroserviceDbContext: DbContext
    {
        public CustomerMicroserviceDbContext(DbContextOptions options): base(options)
        {

        }
        public DbSet<Customer> Customers { get; set; }
    }
}
