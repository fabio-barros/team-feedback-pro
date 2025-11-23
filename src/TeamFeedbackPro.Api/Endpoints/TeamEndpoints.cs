using System.IdentityModel.Tokens.Jwt;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TeamFeedbackPro.Api.Contracts;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Application.Teams.Commands.CreateTeam;
using TeamFeedbackPro.Application.Teams.Commands.DeleteTeam;
using TeamFeedbackPro.Application.Teams.Commands.UpdateTeam;
using TeamFeedbackPro.Application.Teams.Queries.GetAllTeams;
using TeamFeedbackPro.Application.Teams.Queries.GetTeam;

namespace TeamFeedbackPro.Api.Endpoints;

/// <summary>
/// Provides Minimal API endpoints for managing Team entities, including creation, retrieval, update, and deletion.
/// All endpoints are grouped under /api/teams and support OpenAPI documentation.
/// </summary>
public static class TeamEndpoints
{
    /// <summary>
    /// Maps Team CRUD endpoints to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder with Team endpoints mapped.</returns>
    public static IEndpointRouteBuilder MapTeamEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/teams")
            .WithTags("Teams")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapGet("/", GetAllTeams)
            .AllowAnonymous()
            .WithName("GetAllTeams")
            .WithSummary("Get all teams")
            .WithDescription("Retrieves a list of all teams with their members.")
            .Produces<IEnumerable<TeamResult>>(200, contentType: "application/json")
            .WithOpenApi();

        group.MapGet("/{id:guid}", GetTeamById)
            .WithName("GetTeamById")
            .WithSummary("Get team by ID")
            .WithDescription("Retrieves a specific team by its ID.")
            .Produces<TeamResult>(200, contentType: "application/json")
            .ProducesProblem(404)
            .WithOpenApi();

        group.MapPost("/", CreateTeam)
            .RequireAuthorization("AdminOnly") // Only admins can create
            .WithName("CreateTeam")
            .WithSummary("Create a new team")
            .WithDescription("Creates a new team. Requires Admin role.")
            .Produces<TeamResult>(201, contentType: "application/json")
            .ProducesProblem(400)
            .ProducesProblem(403)  
            .ProducesValidationProblem()
            .WithOpenApi(op =>
            {
                op.RequestBody.Description = "Team details including name (required, max 200 characters) and optional manager ID.";
                return op;
            });

        group.MapPut("/{id:guid}", UpdateTeam)
            .RequireAuthorization("ManagerOrAdmin")  
            .WithName("UpdateTeam")
            .WithSummary("Update an existing team")
            .WithDescription("Updates an existing team. Admins can update any team, Managers can update only their own team.")
            .Produces<TeamResult>(200, contentType: "application/json")
            .ProducesProblem(403)
            .ProducesProblem(404)
            .ProducesProblem(400)
            .ProducesValidationProblem()
            .WithOpenApi(op =>
            {
                op.RequestBody.Description = "Updated team details.";
                return op;
            });

        group.MapDelete("/{id:guid}", DeleteTeam)
            .RequireAuthorization("AdminOnly") 
            .WithName("DeleteTeam")
            .WithSummary("Delete a team")
            .WithDescription("Deletes a team. Requires Admin role. Members will have their TeamId set to null.")
            .Produces(204)
            .ProducesProblem(403)
            .ProducesProblem(404)
            .WithOpenApi();

        return group;
    }

    private static async Task<IResult> GetAllTeams(
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetAllTeamsQuery();
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetTeamById(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetTeamQuery(id);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.NotFound(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> CreateTeam(
        [FromBody] CreateTeamRequest request,
        ISender mediator,
        IValidator<CreateTeamCommand> validator,
        CancellationToken cancellationToken)
    {
        var command = new CreateTeamCommand(request.Name, request.ManagerId);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Created($"/api/teams/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> UpdateTeam(
        Guid id,
        [FromBody] UpdateTeamRequest request,
        ISender mediator,
        IValidator<UpdateTeamCommand> validator,
        ClaimsPrincipal user,
        CancellationToken cancellationToken)
    {
        var command = new UpdateTeamCommand(id, request.Name, request.ManagerId);

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        // Check if user is Manager (not Admin)
        var userRole = user.FindFirst(ClaimTypes.Role)?.Value;
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        if (userRole == "Manager" && !string.IsNullOrEmpty(userIdClaim) && Guid.TryParse(userIdClaim, out var userId))
        {
            // Verify manager owns this team
            var teamQuery = new GetTeamQuery(id);
            var teamResult = await mediator.Send(teamQuery, cancellationToken);

            if (teamResult.IsSuccess && teamResult.Value?.ManagerId != userId)
            {
                return Results.Forbid(); 
            }
        }

        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.NotFound(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> DeleteTeam(
        Guid id,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteTeamCommand(id);
        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.NotFound(new { message = result.Error })
            : Results.NoContent();
    }
}