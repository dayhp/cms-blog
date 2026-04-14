using AutoMapper;
using CMSBlog.API.Authorization;
using CMSBlog.API.Services;
using CMSBlog.Core.ConfigOptions;
using CMSBlog.Core.Domain.Identity;
using CMSBlog.Core.Repositories;
using CMSBlog.Core.SeedWorks;
using CMSBlog.Data;
using CMSBlog.Data.Repositories;
using CMSBlog.Data.SeedWorks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace CMSBlog.API.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader());
            });
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        }

        //public static void ConfigureFactory(this IServiceCollection services, string[] args)
        //{
        //    var factory = new ApplicationContextFactory();
        //    using var context = factory.CreateDbContext(args);
        //}

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<,>), typeof(RepositoryBase<,>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<ITokenService, TokenService>();
        }

        public static void ConfigureService(this IServiceCollection services)
        {
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();
        }

        public static void ConfigurationTimeout(this IServiceCollection services)
        {
            services.AddHttpClient("DefaultClient", client =>
            {
                client.Timeout = TimeSpan.FromMinutes(5);
            });
        }

        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        public static void GetConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtTokenSetting>(configuration.GetSection("JwtTokenSettings"));
        }

        public static void ConfigureJwtAuthenticate(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(cfg =>
            {
                cfg.RequireHttpsMetadata = false;
                cfg.SaveToken = true;
                cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtTokenSettings:Issuer"],
                    ValidAudience = configuration["JwtTokenSettings:Audience"],
                    IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["JwtTokenSettings:SecretKey"])),
                    ClockSkew = TimeSpan.Zero
                };
            });
        }

        public static void ConfigureIdentity(this IServiceCollection services)
        {
            services.AddIdentity<AppUser, AppRole>(options => options.SignIn.RequireConfirmedAccount = false)
                .AddEntityFrameworkStores<ApplicationDbContext>();


            services.Configure<IdentityOptions>(options =>
            {
                // password setting
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout setting
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User setting
                options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+/";
                options.User.RequireUniqueEmail = false;
            });
        }
    }
}