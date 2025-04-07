using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorkManagement.Models;

public partial class TaskEmployee
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public int? UserId { get; set; }

    public DateTime? AssignedAt { get; set; }

    public int? StatusId { get; set; }

    public DateTime Deadline { get; set; }

    public DateTime? CompletionDate { get; set; }

    public bool? PenaltyStatus { get; set; }

    
    public virtual StatusJob? Status { get; set; }
    
    public virtual ProjectTask? Task { get; set; }
    
    public virtual User? User { get; set; }
}
