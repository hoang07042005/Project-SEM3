using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class LabUsageLog
{
    public int Id { get; set; }

    public int? LabId { get; set; }

    public int? UserId { get; set; }

    public string Purpose { get; set; } = null!;

    public DateTime StartTime { get; set; }

    public DateTime? EndTime { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Lab? Lab { get; set; }

    public virtual User? User { get; set; }
}
