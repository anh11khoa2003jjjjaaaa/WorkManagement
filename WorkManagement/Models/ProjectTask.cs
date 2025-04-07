using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorkManagement.Models;

public partial class ProjectTask
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public int? ProjectId { get; set; }

    public int? StatusId { get; set; }
    [JsonIgnore]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    public virtual Project? Project { get; set; }

    public virtual StatusJob? Status { get; set; }
    [JsonIgnore]
    public virtual ICollection<TaskEmployee> TaskEmployees { get; set; } = new List<TaskEmployee>();
}
