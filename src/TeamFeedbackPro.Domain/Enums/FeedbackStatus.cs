using System.ComponentModel;

namespace TeamFeedbackPro.Domain.Enums;

public enum FeedbackStatus
{
    [Description("Em análise")]
    Pending,
    [Description("Aprovado")]
    Approved,
    [Description("Rejeitado")]
    Rejected
}