using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorkManagement.Models;

public partial class StatusJob
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    [JsonIgnore]
    public virtual ICollection<ProjectTask> ProjectTasks { get; set; } = new List<ProjectTask>();
    [JsonIgnore]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    [JsonIgnore]
    public virtual ICollection<TaskEmployee> TaskEmployees { get; set; } = new List<TaskEmployee>();
}
