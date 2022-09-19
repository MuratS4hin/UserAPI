using System.Text;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using NSwag;
using UserApi.Middleware;
using UserApi.Middlewares;
using UserApi.Models;
using UserApi.Repository;
using UserApi.Services;
using UserApi.Settings;
using UserApi.Validators;

namespace UserApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<UserDatabaseSettings>(Configuration.GetSection(nameof(UserDatabaseSettings)));

            services.AddSingleton<IUserDatabaseSettings>(sp => sp.GetRequiredService<IOptions<UserDatabaseSettings>>().Value);

            services.Configure<AuthorizeSettings>(Configuration.GetSection(nameof(AuthorizeSettings)));

            services.AddSingleton<IAuthorizeSettings>(sp => sp.GetRequiredService<IOptions<AuthorizeSettings>>().Value);

            services.AddSingleton(x => new DBContext(new MongoClient()));

            services.AddSingleton<DocumentService>();

            services.AddSingleton<UserService>();

            services.AddSingleton<MimeTypes>();

            services.AddTransient<IValidator<RequestedUser>, RequestedUserValidator>();

            services
                .AddMvc(options => { options.EnableEndpointRouting = false; })
                .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<Startup>());

            services.AddSwaggerDocument(c =>
                c.AddSecurity("Bearer", new OpenApiSecurityScheme
                {
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Please insert JWT with Bearer into field",
                    Name = "Authorization",
                    Type = OpenApiSecuritySchemeType.ApiKey
                }));

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = Configuration["AuthorizeSettings:Issuer"],
                        ValidAudience = Configuration["AuthorizeSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["AuthorizeSettings:Key"]))
                    };
                });

            services.AddMvc();

            services.AddControllers();
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:4200").AllowAnyHeader().WithMethods("GET", "PUT", "POST", "DELETE", "UPDATE", "OPTIONS").AllowCredentials();
                    });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseExceptionMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(builder =>
                builder.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod()
                      .AllowCredentials());

            app.UseOpenApi();

            app.UseSwaggerUi3();

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseMvc();

            app.UseOptions();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Could Not Find Anything");
            });
        }
    }
}