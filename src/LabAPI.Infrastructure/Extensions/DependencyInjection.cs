using System.Text;
using LabAPI.Application.Common.Interfaces;
using LabAPI.Application.Features.Accounts.Repository;
using LabAPI.Application.Features.Orders.Repository;
using LabAPI.Application.Features.Tests.Repository;
using LabAPI.Domain.Entities;
using LabAPI.Infrastructure.Authentication;
using LabAPI.Infrastructure.Persistence;
using LabAPI.Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.Azure.Cosmos.Fluent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace LabAPI.Infrastructure.Extensions;

public static class DependencyInjection
{
	public static void AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
	{
		var cosmosClient =
			new CosmosClientBuilder(configuration.GetConnectionString("AzureCosmosDb"))
				.Build();
		services.AddSingleton(cosmosClient);
		services.AddScoped<IOrderRepository, OrderRepository>();
		services.AddScoped<ITestRepository, TestRepository>();
		services.AddDbContext<CosmosDbContext>();
		services.AddScoped<ICustomerRepository, CustomerRepository>();
		services.AddScoped<IWorkerRepository, WorkerRepository>();
		services.AddScoped<IPasswordHasher<Customer>, PasswordHasher<Customer>>();
		services.AddScoped<IPasswordHasher<Worker>, PasswordHasher<Worker>>();
		services.AddScoped<IJwtService, JwtService>();
		var authenticationSettings = new AuthenticationSettings();
		configuration.GetSection("Authentication").Bind(authenticationSettings);

		services.AddAuthentication(option =>
		{
			option.DefaultAuthenticateScheme = "Bearer";
			option.DefaultScheme = "Bearer";
			option.DefaultChallengeScheme = "Bearer";
		}).AddJwtBearer(cfg =>
		{
			cfg.RequireHttpsMetadata = false;
			cfg.SaveToken = true;
			cfg.TokenValidationParameters = new TokenValidationParameters
			{
				ValidIssuer = authenticationSettings.JwtIssuer,
				ValidAudience = authenticationSettings.JwtIssuer,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(authenticationSettings.JwtKey))
			};
		});
		services.AddSingleton(authenticationSettings);
	}
}