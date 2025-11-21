namespace TeamFeedbackPro.Application.Common.Models.FeedbackForm;

public record FeedbackFormDataResult(
    List<KeyValuePair<int, string>> Types,
    List<KeyValuePair<int, string>> Categories,
    List<TeamMemberResult> Users
);
