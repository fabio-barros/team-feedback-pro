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
using TeamFeedbackPro.Application.Sprints.Commands.CreateSprint;

namespace TeamFeedbackPro.Api.Endpoints;

/// <summary>
/// Provides Minimal API endpoints for managing Team entities, including creation, retrieval, update, and deletion.
/// All endpoints are grouped under /api/teams and support OpenAPI documentation.
/// </summary>
public static class SprintEndpoints
{
    /// <summary>
    /// Maps Sprints CRUD endpoints to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder with Team endpoints mapped.</returns>
    public static IEndpointRouteBuilder MapSprintEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sprints")
            .WithTags("Sprints")
            .RequireAuthorization()
            .WithOpenApi();

        group.MapPost("/", CreateSprint)
            .WithName("CreateSprint")
            .WithSummary("Create Sprint")
            .WithDescription("Create sprint for the manager team")
            .RequireAuthorization("ManagerOnly")
            .Produces(201, contentType: "application/json")
            .ProducesProblem(401)
            .WithOpenApi();
 

        return group;
    }

    private static async Task<IResult> CreateSprint(
        [FromBody] CreateSprintRequest request,
        ClaimsPrincipal user,
        ISender mediator,
        IValidator<CreateSprintCommand> validator,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var managerId))
        {
            return Results.Unauthorized();
        }

        var command = new CreateSprintCommand(
            managerId,
            request.Name,
            request.Description,
            request.StartAt,
            request.EndAt
        );

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Created($"/api/sprints/{result.Value.Id}", result.Value);
    }
}