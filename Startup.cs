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
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
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
            #region Settings Defined
            services.Configure<UserDatabaseSettings>(Configuration.GetSection(nameof(UserDatabaseSettings)));

            services.AddSingleton<IUserDatabaseSettings>(sp => sp.GetRequiredService<IOptions<UserDatabaseSettings>>().Value);

            services.Configure<DocumentDatabaseSettings>(Configuration.GetSection(nameof(DocumentDatabaseSettings)));

            services.AddSingleton<IDocumentDatabaseSettings>(sp => sp.GetRequiredService<IOptions<DocumentDatabaseSettings>>().Value);

            services.Configure<AuthorizeSettings>(Configuration.GetSection(nameof(AuthorizeSettings)));

            services.AddSingleton<IAuthorizeSettings>(sp => sp.GetRequiredService<IOptions<AuthorizeSettings>>().Value);
            #endregion

            services.AddSingleton(x => new DBContext(new MongoClient()));

            services.AddSingleton<DocumentService>();

            services.AddSingleton<UserService>();

            services.AddSingleton<MimeTypes>();

            services.AddTransient<IValidator<RequestedUser>, RequestedUserValidator>();

            services
                .AddMvc(options => { options.EnableEndpointRouting = false; })
                .AddFluentValidation(configuration => configuration.RegisterValidatorsFromAssemblyContaining<Startup>());

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


            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });

                // Define the Bearer token security scheme
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please insert JWT token by writing Bearer in front (E.g. Bearer eytxckvds...)",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });

                // Add Bearer token authentication requirement for methods that require authorization
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });
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

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "APIs");
            });

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