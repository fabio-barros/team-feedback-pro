using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Domain.Entities;

public class Feedback : BaseEntity
{
    public Guid AuthorId { get; private set; }
    public Guid RecipientId { get; private set; }
    public FeedbackType Type { get; private set; }
    public FeedbackCategory Category { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public bool IsAnonymous { get; private set; }
    public FeedbackStatus Status { get; private set; }
    public Guid? ReviewedBy { get; private set; }
    public DateTime? ReviewedAt { get; private set; }
    public string? ReviewNotes { get; private set; }
    public Guid TeamId { get; private set; }
    public Guid? FeelingId { get; private set; }

    public virtual User? Author { get; private set; }
    public virtual User? Recipient { get; private set; }
    public virtual User? Reviewer { get; private set; }
    public virtual Team? Team { get; private set; }
    public virtual Feeling? Feeling { get; private set; }

    private Feedback() { } // EF Core

    public Feedback(
        Guid authorId,
        Guid recipientId,
        FeedbackType type,
        FeedbackCategory category,
        string content,
        bool isAnonymous,
        Guid teamId,
        Guid? feelingId=null)
    {
        if (authorId == recipientId)
            throw new ArgumentException("Cannot send feedback to yourself");

        if (string.IsNullOrWhiteSpace(content))
            throw new ArgumentException("Content cannot be empty", nameof(content));

        switch (content.Length)
        {
            case < 20:
                throw new ArgumentException("Content must be at least 20 characters", nameof(content));
            case > 2000:
                throw new ArgumentException("Content must not exceed 2000 characters", nameof(content));
        }

        AuthorId = authorId;
        RecipientId = recipientId;
        Type = type;
        Category = category;
        Content = content.Trim();
        IsAnonymous = isAnonymous;
        Status = FeedbackStatus.Pending;
        TeamId = teamId;
        FeelingId = feelingId;
    }

    public void Approve(Guid reviewerId, string? notes = null)
    {
        if (Status != FeedbackStatus.Pending)
            throw new InvalidOperationException("Only pending feedbacks can be approved");

        Status = FeedbackStatus.Approved;
        ReviewedBy = reviewerId;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Reject(Guid reviewerId, string notes)
    {
        if (Status != FeedbackStatus.Pending)
            throw new InvalidOperationException("Only pending feedbacks can be rejected");

        if (string.IsNullOrWhiteSpace(notes))
            throw new ArgumentException("Rejection notes are required", nameof(notes));

        Status = FeedbackStatus.Rejected;
        ReviewedBy = reviewerId;
        ReviewedAt = DateTime.UtcNow;
        ReviewNotes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}