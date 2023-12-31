﻿//#define MOCKING
using Application.Common.Handler;
using Infrastructure.Handler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using System.Text;
using Application.Common.Persistance;
using Infrastructure.Persistance;
using Infrastructure.Common;
using Infrastructure.Persistance.Mocking;
using Domain.Entity;
using Application.Services;
using Infrastructure.Services;
using Application.Services.Models;
using Microsoft.EntityFrameworkCore;
using Application.Common.Services;
using Infrastructure.Common.Services;
using Infrastructure.Services.ImageUpload;

namespace Infrastructure
{
    public static class RegisterInfrastructureServices
    {
        public static IServiceCollection AddInfrastructure(this  IServiceCollection services, IConfiguration config)
        {
            services.AddTransient<ExceptionHandlingMiddleware>();

            services.AddMemoryCache();
            services.AddSingleton<ICacheService, InMemoryCache>();
            services.AddSingleton<IExchangeRate, ExchangeRates>();

            services.Configure<ExchangeRateConfig>(config.GetSection("ExchangeService"));

            services.AddAuth(config)
                .AddPersistance(config)
                .AddEmailSender(config)
                .AddImageUploader(config);

            return services;
        }
    

    
        private static IServiceCollection AddAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IJwtTokenHander, JwtTokenHandler>();
            services.AddScoped<IPasswordHander, PasswordHandler>();


            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config.GetSection("Secrets:JwtToken").Value)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };
            });

            return services;
        }

        private static IServiceCollection AddPersistance(this IServiceCollection services, IConfiguration config)
        {
#if MOCKING
            services.AddSingleton<MockDb>();
            services.AddScoped<IGenericRepository<Order>, MockOrderRepository>();
            services.AddScoped<IGenericRepository<User>, MockUserRepository>();
            services.AddScoped<IGenericRepository<Product>, MockProductRepository>();
            services.AddScoped<IUnitOfWork, MockUnitOfWork>();
#else
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("DatabaseConnection"));
            });

            services.AddScoped<Func<ApplicationDbContext>>((provider) => provider.GetService<ApplicationDbContext>);

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUnitOfWork, UnitOfWork>();
#endif
            return services;
        }


        private static IServiceCollection AddEmailSender(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<IEmailSender, GmailSender>();

            services.Configure<SMTPSettings>(config.GetSection("SmtpServer"));

            return services;
        }

        private static IServiceCollection AddImageUploader(this IServiceCollection services, IConfiguration config)
        {
            services.AddScoped<IImageService, CloudinaryService>();
            services.Configure<CloudinarySettings>(config.GetSection("ImageService"));

            return services;
        }
    }
}
