using System.ComponentModel;

namespace TeamFeedbackPro.Domain.Enums;

public enum FeedbackType
{
    [Description("Positivo")]
    Positive,
    [Description("Construtivo")]
    Constructive,
    [Description("Crítico")]
    Critical
}