using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorkManagement.Models;

public partial class User
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime? BirthDay { get; set; }

    public string? Avatar { get; set; }

    public int? DepartmentId { get; set; }

    public bool? IsLeader { get; set; }

    public int? PositionId { get; set; }
    [JsonIgnore]
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();

    public virtual Department? Department { get; set; }
    [JsonIgnore]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual Position? Position { get; set; }
    [JsonIgnore]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    [JsonIgnore]
    public virtual ICollection<TaskEmployee> TaskEmployees { get; set; } = new List<TaskEmployee>();
}
