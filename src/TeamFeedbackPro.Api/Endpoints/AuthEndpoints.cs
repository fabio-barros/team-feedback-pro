using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TeamFeedbackPro.Api.Contracts;
using TeamFeedbackPro.Application.Auth.Commands.Register;
using TeamFeedbackPro.Application.Auth.Queries.GetMe;
using TeamFeedbackPro.Application.Auth.Queries.Login;
using TeamFeedbackPro.Application.Common;
using GetMeResult = TeamFeedbackPro.Application.Common.GetMeResult;

namespace TeamFeedbackPro.Api.Endpoints;

public static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication")
            .WithOpenApi();

        group.MapPost("/register", Register)
            .WithName("Register")
            .WithSummary("Register a new user")
            .WithDescription("Creates a new user account with the specified email, password, name, role, and optional team assignment. Returns a JWT token upon successful registration.")
            .Produces<RegisterResult>(200, contentType: "application/json")
            .ProducesProblem(400)
            .ProducesValidationProblem()
            .WithOpenApi(op =>
            {
                op.RequestBody.Description = "User registration details including email (unique), password (min 8 chars, 1 uppercase, 1 lowercase, 1 number), name, role (Member/Manager/Admin), and optional team ID.";
                return op;
            });

        group.MapPost("/login", Login)
            .WithName("Login")
            .WithSummary("Authenticate user")
            .WithDescription("Authenticates a user with email and password. Returns a JWT token and user details on success.")
            .Produces<AuthenticationResult>(200, contentType: "application/json")
            .ProducesProblem(401)
            .WithOpenApi(op =>
            {
                op.RequestBody.Description = "User credentials (email and password). Email is case-insensitive.";
                return op;
            });

        group.MapGet("/me", GetMe)
            .WithName("GetMe")
            .WithSummary("Get current user profile")
            .WithDescription("Retrieves the authenticated user's profile information. Requires a valid JWT token in the Authorization header.")
            .RequireAuthorization()
            .Produces<GetMeResult>(200, contentType: "application/json")
            .ProducesProblem(401)
            .ProducesProblem(404)
            .WithOpenApi(op => op);

        return app;
    }

    private static async Task<IResult> Register(
        [FromBody] RegisterRequest request,
        ISender mediator,
        IValidator<RegisterCommand> validator,
        ILogger<Program> logger,
        CancellationToken cancellationToken)
    {
        var command = new RegisterCommand(
            request.Email,
            request.Password,
            request.Name,
            request.Role,
            request.TeamId
        );

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> Login(
        [FromBody] LoginQuery query,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.Unauthorized()
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetMe(
        ClaimsPrincipal user,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                          ?? user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                          ?? user.FindFirst("sub")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var query = new GetMeQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.NotFound(new { message = result.Error })
            : Results.Ok(result.Value);
    }
}