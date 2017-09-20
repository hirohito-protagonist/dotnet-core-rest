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
using dotnet_core_rest.Services;
using dotnet_core_rest.Helpers;
using dotnet_core_rest.Entities;
using AutoMapper;

namespace dotnet_core_rest
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
            services.AddMvc();
            services.AddDbContext<BookLibraryContext>(options => options.UseInMemoryDatabase("BookLibraryDatabase"));
            services.AddScoped<ILibraryRepository, LibraryRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, BookLibraryContext bookLibraryContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
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
            });

            bookLibraryContext.EnsureSeedDataForContext();
            
            app.UseMvc();
        }
    }
}
