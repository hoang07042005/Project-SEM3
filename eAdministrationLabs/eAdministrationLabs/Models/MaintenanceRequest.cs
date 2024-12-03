using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class MaintenanceRequest
{
    public int Id { get; set; }

    public int? LabId { get; set; }

    public int? ComputerId { get; set; }

    public string Description { get; set; } = null!;

    public string? Status { get; set; }

    public int? RequestedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? ResolvedAt { get; set; }

    public virtual Computer? Computer { get; set; }

    public virtual Lab? Lab { get; set; }

    public virtual User? RequestedByNavigation { get; set; }
}
