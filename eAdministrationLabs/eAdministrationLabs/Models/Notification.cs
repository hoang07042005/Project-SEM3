using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class Notification
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public string Message { get; set; } = null!;

    public string? ReadStatus { get; set; }

    public DateTime? CreatedAt { get; set; }

    public int? RequestId { get; set; }

    public int? LabUsageLogId { get; set; }

    public virtual User User { get; set; } = null!;

    public virtual Request? Request { get; set; }

    public virtual LabUsageLog? LabUsageLog { get; set; }
}
