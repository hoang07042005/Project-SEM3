using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class Computer
{
    public int Id { get; set; }

    public int? LabId { get; set; }

    public string AssetTag { get; set; } = null!;

    public string? Status { get; set; }

    public string? Specifications { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Lab? Lab { get; set; }

    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();
}
