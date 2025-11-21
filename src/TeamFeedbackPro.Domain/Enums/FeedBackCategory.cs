using System.ComponentModel;

namespace TeamFeedbackPro.Domain.Enums;

public enum FeedbackCategory
{
    [Description("Trabalho em equipe")]
    Teamwork,
    [Description("Qualidade de código")]
    CodeQuality,
    [Description("Comunicação")]
    Communication,
    [Description("Resolução de problema")]
    ProblemSolving,
    [Description("Liderança")]
    Leadership,
    [Description("Outro")]
    Other
}