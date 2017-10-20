using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.Formatters.Xml;
using Microsoft.AspNetCore.Http;
using BooksLibrary.Services;
using BooksLibrary.Helpers;
using BooksLibrary.Entities;
using AutoMapper;
using Swashbuckle.AspNetCore.Swagger;
using AspNetCoreRateLimit;

namespace BooksLibrary
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
            services.AddMvc(setupAction => 
            {
                setupAction.ReturnHttpNotAcceptable = true;

                var jsonOutputFormatter = setupAction.OutputFormatters.OfType<JsonOutputFormatter>().FirstOrDefault();

                if (jsonOutputFormatter != null)
                {
                    jsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.hateoas+json");
                }
            }).AddXmlSerializerFormatters();

            services.AddSwaggerGen((c) => 
            {
                c.SwaggerDoc("v1", new Info { Title = "Books Library API", Description = "Library books REST" });
            });

            services.AddDbContext<BookLibraryContext>(options => options.UseInMemoryDatabase("BookLibraryDatabase"));
            services.AddScoped<ILibraryRepository, LibraryRepository>();
            services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();
            services.AddScoped<IUrlHelper>(implementationFactory =>
            {
                var actionContext = implementationFactory.GetService<IActionContextAccessor>()
                .ActionContext;
                return new UrlHelper(actionContext);
            });

            services.AddHttpCacheHeaders((expirationModelOptions) =>
            {
                expirationModelOptions.MaxAge = 600;
            }, 
            (validationModelOptions) =>
            {
                validationModelOptions.AddMustRevalidate = true;
            });
            
            services.AddMemoryCache();

            services.Configure<IpRateLimitOptions>((options) => 
            {
                options.GeneralRules = new List<RateLimitRule>()
                {
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 1000,
                        Period = "5m"
                    },
                    new RateLimitRule()
                    {
                        Endpoint = "*",
                        Limit = 200,
                        Period = "10s"
                    }
                };
            });

            services.AddSingleton<IRateLimitCounterStore, MemoryCacheRateLimitCounterStore>();
            services.AddSingleton<IIpPolicyStore, MemoryCacheIpPolicyStore>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BookLibraryContext bookLibraryContext)
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

            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.Author, Models.AuthorDto>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src =>
                        $"{src.FirstName} {src.LastName}"
                    ))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                        src.DateOfBirth.GetCurrentAge()
                    ));

                cfg.CreateMap<Entities.Book, Models.BookDto>();
                
                cfg.CreateMap<Models.AuthorCreationDto, Entities.Author>(); 

                cfg.CreateMap<Entities.Book, Models.BookManipulationDto>();

                cfg.CreateMap<Models.BookManipulationDto, Entities.Book>();
            });

            bookLibraryContext.EnsureSeedDataForContext();
            
            app.UseIpRateLimiting();
            app.UseHttpCacheHeaders();

            app.UseMvc();

            app.UseSwagger();
            app.UseSwaggerUI((c) => 
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Books Library API");
            });
        }
    }
}
