using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using TeamFeedbackPro.Api.Contracts;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Application.Common.Models.FeedbackForm;
using TeamFeedbackPro.Application.Feedbacks.Commands.ApproveFeedback;
using TeamFeedbackPro.Application.Feedbacks.Commands.CreateFeedback;
using TeamFeedbackPro.Application.Feedbacks.Commands.RejectFeedback;
using TeamFeedbackPro.Application.Feedbacks.Queries.GetFeedbackFormData;
using TeamFeedbackPro.Application.Feedbacks.Queries.GetPendingFeedbacks;
using TeamFeedbackPro.Application.Feedbacks.Queries.GetReceivedFeedbacks;
using TeamFeedbackPro.Application.Feedbacks.Queries.GetSentFeedbacks;
using TeamFeedbackPro.Application.Users.Queries.GetTeamMembers;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Api.Endpoints;

/// <summary>
/// Provides Minimal API endpoints for managing Feedback entities.
/// All endpoints are grouped under /api/feedbacks and support OpenAPI documentation.
/// </summary>
public static class FeedbackEndpoints
{
    /// <summary>
    /// Maps Feedback endpoints to the application route builder.
    /// </summary>
    /// <param name="app">The endpoint route builder.</param>
    /// <returns>The endpoint route builder with Feedback endpoints mapped.</returns>
    public static IEndpointRouteBuilder MapFeedbackEndpoints(this IEndpointRouteBuilder app)
    {
        var feedbackGroup = app.MapGroup("/api/feedbacks")
            .WithTags("Feedbacks")
            .RequireAuthorization()
            .WithOpenApi();

        feedbackGroup.MapPost("/", CreateFeedback)
            .WithName("CreateFeedback")
            .WithSummary("Create a new feedback")
            .WithDescription("Creates a new feedback for a team member. Author and recipient must be in the same team.")
            .Produces<FeedbackResult>(201, contentType: "application/json")
            .ProducesProblem(400)
            .ProducesProblem(403)
            .ProducesValidationProblem()
            .WithOpenApi(op =>
            {
                op.RequestBody.Description = "Feedback details including recipient, type, category, content (20-2000 chars), and anonymity flag.";
                return op;
            });

        feedbackGroup.MapGet("/sent", GetSentFeedbacks)
            .WithName("GetSentFeedbacks")
            .WithSummary("Get sent feedbacks")
            .WithDescription("Retrieves feedbacks sent by the authenticated user with optional filtering and pagination.")
            .Produces<PaginatedResult<FeedbackResult>>(200, contentType: "application/json")
            .ProducesProblem(401)
            .WithOpenApi();
 
        feedbackGroup.MapGet("/received", GetReceivedFeedbacks)
            .WithName("GetReceivedFeedbacks")
            .WithSummary("Get receveid feedbacks")
            .WithDescription("Retrieves feedbacks received, approved by manager, by the authenticated user with optional filtering and pagination.")
            .Produces<PaginatedResult<FeedbackResult>>(200, contentType: "application/json")
            .ProducesProblem(401)
            .WithOpenApi();

        feedbackGroup.MapGet("/review-peding", GetPendingFeedbacks)
            .WithName("GetPedingFeedbacks")
            .WithSummary("Get pending feedbacks")
            .WithDescription("Retrieves feedbacks pending for review by the manager with optional filtering and pagination.")
            .RequireAuthorization("ManagerOnly")
            .Produces<PaginatedResult<FeedbackResult>>(200, contentType: "application/json")
            .ProducesProblem(401)
            .WithOpenApi();

        feedbackGroup.MapGet("/feedback-form-data", GetFeedbackFormData)
            .WithName("GetFeedbackFormData")
            .WithSummary("Get data for feedback form")
            .WithDescription("Retrieves necessaries data for send a feedback")
            .Produces<FeedbackFormDataResult>(200, contentType: "application/json")
            .ProducesProblem(401)
            .WithOpenApi();

        feedbackGroup.MapPatch("/approve", ApproveFeedback)
            .WithName("ApproveFeedback")
            .WithSummary("Approve feedback")
            .WithDescription("Approve feedback by manager on the pending feedbacks of them team")
            .RequireAuthorization("ManagerOnly")
            .Produces<FeedbackFormDataResult>(200, contentType: "application/json")
            .ProducesProblem(401)
            .WithOpenApi();

        feedbackGroup.MapPatch("/reject", RejectFeedback)
            .WithName("RejectFeedback")
            .WithSummary("Reject feedback")
            .WithDescription("Reject feedback by manager on the pending feedbacks of them team")
            .RequireAuthorization("ManagerOnly")
            .Produces(201, contentType: "application/json")
            .ProducesProblem(401)
            .WithOpenApi();

        var usersGroup = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization()
            .WithOpenApi();

        usersGroup.MapGet("/team-members", GetTeamMembers)
            .WithName("GetTeamMembers")
            .WithSummary("Get team members")
            .WithDescription("Retrieves all members of the authenticated user's team (excluding the user themselves).")
            .Produces<IEnumerable<TeamMemberResult>>(200, contentType: "application/json")
            .WithOpenApi();

        return app;
    }

    private static async Task<IResult> CreateFeedback(
        [FromBody] CreateFeedbackRequest request,
        ClaimsPrincipal user,
        ISender mediator,
        IValidator<CreateFeedbackCommand> validator,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var authorId))
        {
            return Results.Unauthorized();
        }

        var command = new CreateFeedbackCommand(
            authorId,
            request.RecipientId,
            request.Type,
            request.Category,
            request.Content,
            request.IsAnonymous,
            request.FeelingId,
            request.ImprovementSuggestion
        );

        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());

        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Created($"/api/feedbacks/{result.Value.Id}", result.Value);
    }

    private static async Task<IResult> GetSentFeedbacks(
        ClaimsPrincipal user,
        ISender mediator,
        [FromQuery] FeedbackStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var authorId))
        {
            return Results.Unauthorized();
        }

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var query = new GetSentFeedbacksQuery(authorId, status, page, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetReceivedFeedbacks(
        ClaimsPrincipal user,
        ISender mediator,
        [FromQuery] FeedbackStatus? status,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var query = new GetReceivedFeedbacksQuery(userId, status, page, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetPendingFeedbacks(
        ClaimsPrincipal user,
        ISender mediator,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var teamIdClaim = user.FindFirst("teamId")?.Value;
        if (!Guid.TryParse(teamIdClaim, out var teamId))
        {
            return Results.BadRequest("Gerente não vinculado à um time");
        }

        if (page < 1) page = 1;
        if (pageSize < 1 || pageSize > 100) pageSize = 10;

        var query = new GetPendingFeedbacksQuery(userId, teamId, page, pageSize);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetTeamMembers(
        ClaimsPrincipal user,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var query = new GetTeamMembersQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetFeedbackFormData(
        ClaimsPrincipal user,
        ISender mediator,
        CancellationToken cancellationToken)
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        var query = new GetFeedbackFormDataQuery(userId);
        var result = await mediator.Send(query, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> ApproveFeedback(
        ClaimsPrincipal user,
        ISender mediator,
        [FromQuery] Guid feedbackId,
        ReviewFeedbackRequest? reviewFeedbackRequest,
        IValidator<ApproveFeedbackCommand> validator,
        CancellationToken cancellationToken
    )
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        if (feedbackId == Guid.Empty)
        {
            return Results.BadRequest("Não foi recebido um feedback para aprovar");
        }

        var command = new ApproveFeedbackCommand(feedbackId, userId, reviewFeedbackRequest?.Review);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());
        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> RejectFeedback(
        ClaimsPrincipal user,
        ISender mediator,
        [FromQuery] Guid feedbackId,
        ReviewFeedbackRequest reviewFeedbackRequest,
        IValidator<RejectFeedbackCommand> validator,
        CancellationToken cancellationToken
    )
    {
        var userIdClaim = user.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
        if (!Guid.TryParse(userIdClaim, out var userId))
        {
            return Results.Unauthorized();
        }

        if (feedbackId == Guid.Empty)
        {
            return Results.BadRequest("Não foi recebido um feedback para aprovar");
        }

        var command = new RejectFeedbackCommand(feedbackId, userId, reviewFeedbackRequest.Review);
        var validationResult = await validator.ValidateAsync(command, cancellationToken);
        if (!validationResult.IsValid)
            return Results.ValidationProblem(validationResult.ToDictionary());
        
        var result = await mediator.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.BadRequest(new { message = result.Error })
            : Results.Ok(result.Value);
    }
}