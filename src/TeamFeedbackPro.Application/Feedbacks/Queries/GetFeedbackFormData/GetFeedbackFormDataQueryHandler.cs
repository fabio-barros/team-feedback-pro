using MediatR;
using Microsoft.Extensions.Logging;
using TeamFeedbackPro.Application.Common;
using TeamFeedbackPro.Application.Common.Abstractions;
using TeamFeedbackPro.Application.Common.Models;
using TeamFeedbackPro.Application.Common.Models.FeedbackForm;
using TeamFeedbackPro.Application.Users.Queries.GetTeamMembers;
using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Application.Feedbacks.Queries.GetFeedbackFormData;

public class GetFeedbackFormDataQueryHandler(
    ISender mediator,
    IFeelingRepository feelingRepository,
    IUserRepository userRepository,
    ISprintRepository sprintRepository,
    ILogger<GetFeedbackFormDataQueryHandler> logger)
    : IRequestHandler<GetFeedbackFormDataQuery, Result<FeedbackFormDataResult>>
{
    public async Task<Result<FeedbackFormDataResult>> Handle(GetFeedbackFormDataQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting feedback form data");

        var query = new GetTeamMembersQuery(request.AuthorId);
        var result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure || result.Value == null)
            return Result.Failure<FeedbackFormDataResult>(result.Error ?? ErrorMessages.TeamMembersNotFound);

        var teamMembers = result.Value.ToList();

        List<KeyValuePair<int, string>> feedbackTypes = [.. Enum.GetValues(typeof(FeedbackType))
            .Cast<FeedbackType>()
            .Select(p => new KeyValuePair<int, string>((int)p, p.ToDescription()))];

        List<KeyValuePair<int, string>> feedbackCategories = [.. Enum.GetValues(typeof(FeedbackCategory))
            .Cast<FeedbackCategory>()
            .Select(p => new KeyValuePair<int, string>((int)p, p.ToDescription()))];

        var feelings = (await feelingRepository.GetAllAsync()).ToList();
        List<KeyValuePair<Guid, string>> feelingsKeyValue = [.. feelings.Select(f => new KeyValuePair<Guid,string>(f.Id, f.Name))];

        var sprintName = "Não encontrada";
        var author = await userRepository.GetByIdAsync(request.AuthorId);
        if (author != null && author.TeamId.HasValue)
        {
            var sprint = await sprintRepository.GetActualSprintAsync(author.TeamId.Value);
            if (sprint != null)
            {
                sprintName = sprint.Name;
            }
        }

        var formData = new FeedbackFormDataResult(feedbackTypes, feedbackCategories, teamMembers, feelingsKeyValue, sprintName);
        logger.LogInformation("Retrieved form data");

        return Result.Success(formData);
    }
}