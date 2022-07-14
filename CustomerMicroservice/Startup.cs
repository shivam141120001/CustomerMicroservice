using CustomerMicroservice.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using CustomerMicroservice.Context;
using CustomerMicroservice.Models;
using Microsoft.OpenApi.Models;
using CustomerMicroservice.Middleware;
using System.Reflection;
using System.IO;

namespace CustomerMicroservice
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddScoped<ICustomerRepository, CustomerRepository>();
            services.AddScoped<IAuthMiddleware, AuthMiddleware>();
            services.AddDbContext<CustomerMicroserviceDbContext>(option => option.UseInMemoryDatabase(Configuration.GetConnectionString("connstr")));

          /*  services.AddSwaggerGen(c => c.SwaggerDoc(name: "v1.0", new OpenApiInfo { Title = "Customer Microservice ", Version = "1.0"
            
            }));*/
          

            services.AddCors();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env,ILoggerFactory loggerFactory)
        {
            app.UseCors(options => options.WithOrigins("http://localhost:4200").AllowAnyMethod().AllowAnyHeader());
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            loggerFactory.AddLog4Net();
/*
            app.UseSwagger(c =>
            {
                c.SerializeAsV2 = true;
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(url: "/swagger/v1.0/swagger.json", "Customer Microservice (V 1.0)");
                c.RoutePrefix = string.Empty;
            });
*/

            
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

          
            var context = app.ApplicationServices.CreateScope().ServiceProvider.GetService<CustomerMicroserviceDbContext>();
            SeedData(context);
        }
        public static void SeedData(CustomerMicroserviceDbContext context)
        {
            context.Customers.AddRange(new Customer[]
            {
                new Customer() { Name = "Ayush", Address = "Delhi", EmailId = "ayush@gmail.com", ContactNumber = 7845203698, ExecutiveId=1, Username="ayush", Password="ayushPass" },
                new Customer() { Name = "Aditya", Address = "Gurugram", EmailId = "aditya@gmail.com", ContactNumber = 9845208865, ExecutiveId=2, Username="aditya", Password="adityaPass" },
                new Customer() { Name = "Naman", Address = "Meerut", EmailId = "naman@gmail.com", ContactNumber = 8944508865, ExecutiveId=0, Username="naman", Password="namanPass" },
                new Customer() { Name = "Disha", Address = "Kanpur", EmailId = "disha@gmail.com", ContactNumber = 7094508860, ExecutiveId=3, Username="disha", Password="dishaPass" },
                new Customer() { Name = "Gaurav", Address = "Pune", EmailId = "gaurav@gmail.com", ContactNumber = 7804508857, ExecutiveId=4, Username="gaurav", Password="gauravPass" },
                new Customer() { Name = "Vamshi", Address = "Hyderabad", EmailId = "vamshi@gmail.com", ContactNumber = 9904508857, ExecutiveId=0, Username="vamshi", Password="vamshiPass" }
            });
            context.SaveChanges();
        }
    }
}
