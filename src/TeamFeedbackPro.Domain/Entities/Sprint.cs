namespace TeamFeedbackPro.Domain.Entities;
public class Sprint : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public DateTime StartAt { get; private set; }
    public DateTime EndAt { get; private set; }
    public Guid TeamId { get; private set; }

    // // Navigation Properties
    // private readonly List<Feedback> _feedbacks = [];
    // public virtual IReadOnlyCollection<Feedback> Feedbacks => _feedbacks.AsReadOnly();

    private Sprint() { } // EF Core

    public Sprint(string name, string? description, DateTime startAt, DateTime endAt, Guid teamId)
    {
        Name = name;
        Description = description;
        StartAt = startAt;
        EndAt = endAt;
        TeamId = teamId;
    }
}