using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Users.API.Entities;
using Users.API.Models;
using Users.API.Services;

namespace Users.API
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
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddSingleton<IUsersRepository, UsersRepository>();
            services.AddAutoMapper(typeof(Startup));

            services.AddSwaggerGen(setupAction =>
            {
                setupAction.SwaggerDoc("UserOpenAPISpecification", new OpenApiInfo()
                {
                    Title = "User API",
                    Version = "1",
                    Description = "Through this API you can access users.",
                    Contact = new OpenApiContact()
                    {
                        Email = "me@jasonmorsley.dev",
                        Name = "Jason Morsley",
                        //Url = new Uri("https://www.jasonmorsley.dev")
                    },
                    //License = new OpenApiLicense()
                    //{
                    //    Name = "MIT License",
                    //    Url = new Uri("https://opensource.org/licenses/MIT")
                    //}
                });

                var xmlCommentsFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlCommentsFullPath = Path.Combine(AppContext.BaseDirectory, xmlCommentsFile);

                setupAction.IncludeXmlComments(xmlCommentsFullPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory) //, UserContext userContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An unexpected fault happened. Try again later.");
                    });
                });
            }

            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<User, UserDto>()
                    .ForMember(destination => destination.Name, options => options.MapFrom(src =>
                        $"{src.FirstName} {src.LastName}"));

                cfg.CreateMap<UserForCreationDto, User>();
            });

            //IMapper iMapper = config.CreateMapper();
            //var source = new User();
            //var destination = iMapper.Map<User, UserDto>(source);
            
            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(setupAction =>
            {
                setupAction.SwaggerEndpoint("/swagger/UserOpenAPISpecification/swagger.json", "User API");
                setupAction.RoutePrefix = "";
            });

            app.UseMvc();
        }
    }
}
