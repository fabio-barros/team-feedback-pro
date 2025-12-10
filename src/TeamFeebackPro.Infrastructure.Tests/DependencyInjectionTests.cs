using FluentAssertions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedBackPro.Infrastructure;
using TeamFeedBackPro.Infrastructure.Authentication;
using TeamFeedBackPro.Infrastructure.Persistence;
using TeamFeedBackPro.Infrastructure.Persistence.Repositories;

namespace TeamFeebackPro.Infrastructure.Tests;

public class DependencyInjectionTests
{
    private IServiceCollection CreateServiceCollection()
    {
        var services = new ServiceCollection();

        // Add configuration with test values
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Host=localhost;Database=testdb;Username=test;Password=test" },
                { "JwtSettings:Secret", "12345678901234567890123456789012" },
                { "JwtSettings:Issuer", "TestIssuer" },
                { "JwtSettings:Audience", "TestAudience" },
                { "JwtSettings:ExpiryDays", "7" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();

        return services;
    }

    [Fact]
    public void AddInfrastructure_RegistersAllServices()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<ApplicationDbContext>().Should().NotBeNull();
        serviceProvider.GetService<IUnitOfWork>().Should().NotBeNull();
        serviceProvider.GetService<IUserRepository>().Should().NotBeNull();
        serviceProvider.GetService<ITeamRepository>().Should().NotBeNull();
        serviceProvider.GetService<IFeelingRepository>().Should().NotBeNull();
        serviceProvider.GetService<IFeedbackRepository>().Should().NotBeNull();
        serviceProvider.GetService<ISprintRepository>().Should().NotBeNull();
        serviceProvider.GetService<IJwtTokenGenerator>().Should().NotBeNull();
        serviceProvider.GetService<IPasswordHasher>().Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_RegistersDbContext()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var dbContext = serviceProvider.GetService<ApplicationDbContext>();
        dbContext.Should().NotBeNull();
        dbContext.Should().BeOfType<ApplicationDbContext>();
    }

    [Fact]
    public void AddInfrastructure_RegistersRepositoriesAsScoped()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var repo1 = scope1.ServiceProvider.GetRequiredService<IUserRepository>();
        var repo2 = scope1.ServiceProvider.GetRequiredService<IUserRepository>();
        var repo3 = scope2.ServiceProvider.GetRequiredService<IUserRepository>();

        repo1.Should().BeSameAs(repo2); // Same scope
        repo1.Should().NotBeSameAs(repo3); // Different scope
    }

    [Fact]
    public void AddInfrastructure_RegistersJwtTokenGeneratorAsSingleton()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var instance1 = serviceProvider.GetRequiredService<IJwtTokenGenerator>();
        var instance2 = serviceProvider.GetRequiredService<IJwtTokenGenerator>();

        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void AddInfrastructure_RegistersPasswordHasherAsSingleton()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var instance1 = serviceProvider.GetRequiredService<IPasswordHasher>();
        var instance2 = serviceProvider.GetRequiredService<IPasswordHasher>();

        instance1.Should().BeSameAs(instance2);
    }

    [Fact]
    public void AddInfrastructure_ConfiguresJwtSettings()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var jwtSettings = serviceProvider.GetRequiredService<IOptions<JwtSettings>>().Value;
        jwtSettings.Should().NotBeNull();
        jwtSettings.Secret.Should().Be("12345678901234567890123456789012");
        jwtSettings.Issuer.Should().Be("TestIssuer");
        jwtSettings.Audience.Should().Be("TestAudience");
        jwtSettings.ExpiryDays.Should().Be(7);
    }

    [Fact]
    public void AddInfrastructure_ConfiguresAuthentication()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authOptions = serviceProvider.GetService<IOptions<Microsoft.AspNetCore.Authentication.AuthenticationOptions>>();
        authOptions.Should().NotBeNull();
        authOptions!.Value.DefaultAuthenticateScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
        authOptions.Value.DefaultChallengeScheme.Should().Be(JwtBearerDefaults.AuthenticationScheme);
    }

    [Fact]
    public void AddInfrastructure_ConfiguresAuthorizationPolicies()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authOptions = serviceProvider.GetService<IOptions<Microsoft.AspNetCore.Authorization.AuthorizationOptions>>();
        authOptions.Should().NotBeNull();

        var policies = authOptions!.Value;
        policies.GetPolicy("AdminOnly").Should().NotBeNull();
        policies.GetPolicy("ManagerOrAdmin").Should().NotBeNull();
        policies.GetPolicy("ManagerOnly").Should().NotBeNull();
    }

    [Fact]
    public void AddInfrastructure_AdminOnlyPolicy_RequiresAdminRole()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authOptions = serviceProvider.GetRequiredService<IOptions<Microsoft.AspNetCore.Authorization.AuthorizationOptions>>();
        var policy = authOptions.Value.GetPolicy("AdminOnly");

        policy.Should().NotBeNull();
        policy!.Requirements.Should().HaveCount(1);
    }

    [Fact]
    public void AddInfrastructure_ManagerOrAdminPolicy_RequiresManagerOrAdminRole()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var authOptions = serviceProvider.GetRequiredService<IOptions<Microsoft.AspNetCore.Authorization.AuthorizationOptions>>();
        var policy = authOptions.Value.GetPolicy("ManagerOrAdmin");

        policy.Should().NotBeNull();
        policy!.Requirements.Should().HaveCount(1);
    }

    [Fact]
    public void AddInfrastructure_WithMissingConnectionString_ThrowsException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "JwtSettings:Secret", "12345678901234567890123456789012" },
                { "JwtSettings:Issuer", "TestIssuer" },
                { "JwtSettings:Audience", "TestAudience" },
                { "JwtSettings:ExpiryDays", "7" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Act
        var act = () => services.AddInfrastructure(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Connection string 'DefaultConnection' not found*");
    }

    [Fact]
    public void AddInfrastructure_WithMissingJwtSettings_ThrowsException()
    {
        // Arrange
        var services = new ServiceCollection();
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                { "ConnectionStrings:DefaultConnection", "Host=localhost;Database=testdb;Username=test;Password=test" }
            })
            .Build();

        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();

        // Act
        var act = () => services.AddInfrastructure(configuration);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Configuration section 'JwtSettings' is missing*");
    }

    [Fact]
    public void AddInfrastructure_RegistersUnitOfWorkAsScoped()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        using var scope1 = serviceProvider.CreateScope();
        using var scope2 = serviceProvider.CreateScope();

        var uow1 = scope1.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var uow2 = scope1.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var uow3 = scope2.ServiceProvider.GetRequiredService<IUnitOfWork>();

        uow1.Should().BeSameAs(uow2);
        uow1.Should().NotBeSameAs(uow3);
    }

    [Fact]
    public void AddInfrastructure_ConfiguresJwtBearerOptions()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        var jwtOptions = serviceProvider.GetService<IOptionsMonitor<JwtBearerOptions>>();
        jwtOptions.Should().NotBeNull();

        var options = jwtOptions!.Get(JwtBearerDefaults.AuthenticationScheme);
        options.TokenValidationParameters.ValidateIssuer.Should().BeTrue();
        options.TokenValidationParameters.ValidateAudience.Should().BeTrue();
        options.TokenValidationParameters.ValidateLifetime.Should().BeTrue();
        options.TokenValidationParameters.ValidateIssuerSigningKey.Should().BeTrue();
        options.TokenValidationParameters.ValidIssuer.Should().Be("TestIssuer");
        options.TokenValidationParameters.ValidAudience.Should().Be("TestAudience");
        options.TokenValidationParameters.ClockSkew.Should().Be(TimeSpan.Zero);
    }

    [Fact]
    public void AddInfrastructure_RegistersAllRepositoryInterfaces()
    {
        // Arrange
        var services = CreateServiceCollection();

        // Act
        services.AddInfrastructure(services.BuildServiceProvider().GetRequiredService<IConfiguration>());
        var serviceProvider = services.BuildServiceProvider();

        // Assert
        serviceProvider.GetService<IUserRepository>().Should().BeOfType<UserRepository>();
        serviceProvider.GetService<ITeamRepository>().Should().BeOfType<TeamRepository>();
        serviceProvider.GetService<IFeelingRepository>().Should().BeOfType<FeelingRepository>();
        serviceProvider.GetService<IFeedbackRepository>().Should().BeOfType<FeedbackRepository>();
        serviceProvider.GetService<ISprintRepository>().Should().BeOfType<SprintRepository>();
    }
}