namespace TeamFeedbackPro.Domain.Entities;
public class Feeling : BaseEntity
{
    public string Name { get; private set; } = string.Empty;

    // Navigation Properties
    private readonly List<Feedback> _feedbacks = [];
    public virtual IReadOnlyCollection<Feedback> Feedbacks => _feedbacks.AsReadOnly();

    private Feeling() { } // EF Core

}