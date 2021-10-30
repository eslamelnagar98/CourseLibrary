using CourseLibrary.API.DbContexts;
using CourseLibrary.API.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Serilog;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;

namespace CourseLibrary.API
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
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddControllers(setupAcceptable =>
            {
                setupAcceptable.ReturnHttpNotAcceptable = true;
            })
                    .AddNewtonsoftJson(setupAction =>
                    {
                        setupAction.SerializerSettings.ContractResolver =
                        new CamelCasePropertyNamesContractResolver();
                    })
                    .AddXmlDataContractSerializerFormatters()
                    .ConfigureApiBehaviorOptions(setupAction =>
                    {
                        setupAction.InvalidModelStateResponseFactory = context =>
                         {
                             var problemDetailsFactory = context.HttpContext.RequestServices
                                 .GetRequiredService<ProblemDetailsFactory>();
                             var problemDetails = problemDetailsFactory.CreateValidationProblemDetails
                             (context.HttpContext,
                              context.ModelState);

                             problemDetails.Detail = "See The errors field for details";
                             problemDetails.Instance = context.HttpContext.Request.Path;

                             var actionExecutingContext =
                                 context as ActionExecutingContext;
                             if ((context.ModelState.ErrorCount > 0) &&
                                 actionExecutingContext?.ActionArguments.Count ==
                                 context.ActionDescriptor.Parameters.Count)
                             {
                                 problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                                 problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                                 problemDetails.Title = "One or More validators errors occurred ";

                                 return new UnprocessableEntityObjectResult(problemDetails)
                                 {
                                     ContentTypes = { "application/problem+json" }
                                 };
                             }
                             problemDetails.Status = StatusCodes.Status400BadRequest;
                             problemDetails.Title = "One or More errors on input occured ";

                             return new BadRequestObjectResult(problemDetails)
                             {
                                 ContentTypes = { "application/problem+json" }
                             };

                         };
                    });

            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();
            services.AddDbContext<CourseLibraryContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ApiPluralSight"))
                       .LogTo(Console.WriteLine,
                        new[] { DbLoggerCategory.Database.Command.Name },
                        LogLevel.Information);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseSerilogRequestLogging();
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

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
