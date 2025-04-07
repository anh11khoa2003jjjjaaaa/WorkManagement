using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorkManagement.Models;

public partial class Position
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }
    [JsonIgnore]
    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
