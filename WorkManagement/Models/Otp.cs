using System;
using System.Collections.Generic;

namespace WorkManagement.Models;

public partial class Otp
{
    public int Id { get; set; }

    public int? AccountId { get; set; }

    public string Otpcode { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime ExpiryAt { get; set; }

    public bool? IsUsed { get; set; }

    public virtual Account? Account { get; set; }
}
