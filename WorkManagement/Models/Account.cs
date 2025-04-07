using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WorkManagement.Models;

public partial class Account
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public int? UserId { get; set; }

    public int? RoleId { get; set; }

    public bool? IsActive { get; set; }

    [JsonIgnore]
    public virtual ICollection<Otp> Otps { get; set; } = new List<Otp>();

    public virtual Role? Role { get; set; }

    public virtual User? User { get; set; }
}
