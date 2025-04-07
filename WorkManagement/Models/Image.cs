using System;
using System.Collections.Generic;

namespace WorkManagement.Models;

public partial class Image
{
    public int Id { get; set; }

    public string FilePath { get; set; } = null!;

    public int? TaskId { get; set; }

    public int? ProjectId { get; set; }

    public virtual Project? Project { get; set; }

    public virtual ProjectTask? Task { get; set; }
}
