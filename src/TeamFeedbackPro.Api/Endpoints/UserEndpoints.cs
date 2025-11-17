using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamFeedbackPro.Api.Contracts;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Application.Users.Commands.DeleteUser;
using TeamFeedbackPro.Application.Users.Commands.UpdatePassword;
using TeamFeedbackPro.Application.Users.Commands.UpdateUser;
using TeamFeedbackPro.Application.Users.Queries.GetAllUsers;
using TeamFeedbackPro.Application.Users.Queries.GetUser;

namespace TeamFeedbackPro.Api.Endpoints;

/// <summary>
/// Provides Minimal API endpoints for managing User entities, including retrieval, update, and deletion.
/// All endpoints are grouped under /api/users and support OpenAPI documentation.
/// </summary>
public static class UserEndpoints
{
    /// <summary>
    /// Maps User CRUD endpoints to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder with User endpoints mapped.</returns>
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetAllUsers)
            .WithName("GetAllUsers")
            .WithSummary("Get all users")
            .WithDescription("Retrieves a list of all users. Requires authentication.")
            .Produces<IEnumerable<UserResult>>(200, contentType: "application/json")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetUserById)
            .WithName("GetUserById")
            .WithSummary("Get user by ID")
            .WithDescription("Retrieves a specific user by their ID. Requires authentication.")
            .Produces<UserResult>(200, contentType: "application/json")
            .ProducesProblem(404)
            .WithOpenApi();

        group.MapPut("/{id:guid}", UpdateUser)
            .RequireAuthorization("AdminOnly")
            .WithName("UpdateUser")
            .WithSummary("Update an existing user")
            .WithDescription("Updates an existing user's name, role, and team assignment. Requires Admin role.")
            .Produces<UserResult>(200, contentType: "application/json")
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesValidationProblem()
            .WithOpenApi(op =>
            {
                op.RequestBody.Description = "Updated user details.";
                return op;
            });

        group.MapPut("/{id:guid}/password", UpdatePassword)
            .WithName("UpdatePassword")
            .WithSummary("Update user password")
            .WithDescription("Updates the user's password. Users can only update their own password unless they are Admin.")
            .Produces<bool>(200, contentType: "application/json")
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesValidationProblem()
            .WithOpenApi(op =>
            {
                op.RequestBody.Description = "Current and new password.";
                return op;
            });

        group.MapDelete("/{id:guid}", DeleteUser)
            .RequireAuthorization("AdminOnly")
            .WithName("DeleteUser")
            .WithSummary("Delete a user")
            .WithDescription("Deletes a user by ID. Requires Admin role.")
            .Produces(204)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .WithOpenApi();

        return group;
    }

    private static async Task<IResult> GetAllUsers(
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAllUsersQuery();
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetUserById(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUserQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.NotFound(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdateUser(
        Guid id,
        [FromBody] UpdateUserRequest request,
        ISender mediator,
        IValidator<UpdateUserCommand> validator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(id, request.Name, request.Role, request.TeamId);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.NotFound(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> UpdatePassword(
        Guid id,
        [FromBody] UpdatePasswordRequest request,
        ClaimsPrincipal user,
        ISender mediator,
        IValidator<UpdatePasswordCommand> validator,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;

        // Only allow users to update their own password unless they are Admin
        if (userRole != "Admin" && (!Guid.TryParse(userIdClaim, out var currentUserId) || currentUserId != id))
        {
            return Results.Forbid();
        }

        var command = new UpdatePasswordCommand(id, request.CurrentPassword, request.NewPassword);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteUser(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.NotFound(new { message = result.Error })
            : Results.NoContent();
    }
}