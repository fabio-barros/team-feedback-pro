
using System;
using System.Collections.Generic;
namespace TeamFeedbackPro.Domain.Entities;
public class Team : BaseEntity
{
    public string Name { get; private set; }
    public Guid? ManagerId { get; private set; }

    // Navigation Properties
    private readonly List<User> _members = [];
    public virtual IReadOnlyCollection<User> Members => _members.AsReadOnly();

    private Team() { } // EF Core

    public Team(string name, Guid? managerId = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Team name cannot be empty", nameof(name));

        Name = name.Trim();
        ManagerId = managerId;
    }

    public void UpdateName(string newName)
    {
        if (string.IsNullOrWhiteSpace(newName))
            throw new ArgumentException("Team name cannot be empty", nameof(newName));

        Name = newName.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void AssignManager(Guid managerId)
    {
        ManagerId = managerId;
        UpdatedAt = DateTime.UtcNow;
    }
}