using TeamFeedbackPro.Domain.Enums;

namespace TeamFeedbackPro.Domain.Entities;

public class User : BaseEntity
{
    public string Email { get; private set; }
    public string PasswordHash { get; private set; }
    public string Name { get; private set; }
    public UserRole Role { get; private set; }
    public Guid? TeamId { get; private set; }

    // Navigation Properties
    public virtual Team? Team { get; private set; }

    private User() { } // EF Core

    public User(string email, string passwordHash, string name, UserRole role, Guid? teamId = null)
    {
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email cannot be empty", nameof(email));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(passwordHash));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty", nameof(name));

        Email = email.ToLowerInvariant().Trim();
        PasswordHash = passwordHash;
        Name = name.Trim();
        Role = role;
        TeamId = teamId;
    }

    public void UpdatePassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash))
            throw new ArgumentException("Password hash cannot be empty", nameof(newPasswordHash));

        PasswordHash = newPasswordHash;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRole(UserRole newRole)
    {
        Role = newRole;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignToTeam(Guid teamId)
    {
        TeamId = teamId;
        UpdatedAt = DateTime.UtcNow;
    }
}