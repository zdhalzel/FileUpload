using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Azure;
using FileUpload.Models;
using Microsoft.EntityFrameworkCore;
using FileUpload.Hubs;
using FileUpload.Mongo;

namespace FileUpload
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
            services.AddRazorPages();

            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["halzelstoragesecret"]); //.WithName("storageA"); // TODO 
            });

            services.AddAzureClients(builder =>
            {
                builder.AddBlobServiceClient(Configuration["halzelstoragesecretB"]).WithName("storageB"); // TODO 
            });

            //services.AddAzureClients(builder =>
            //{
            //    builder.AddBlobServiceClient(Configuration["halzelstoragesecretB"]); // TODO 
            //});

            services.AddDbContext<MyFileContext>(options =>
            options.UseSqlServer(Configuration["halzeldbsecret"]));

            services.AddSignalR().AddAzureSignalR(Configuration["halzelsignalr"]);
            services.AddSingleton<MongoHelper>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseFileServer();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapHub<ChatHub>("/chatHub");
            });
        }
    }
}
