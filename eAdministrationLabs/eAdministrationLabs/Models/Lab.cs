using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class Lab
{
    public int Id { get; set; }

    public string LabName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int Capacity { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Computer> Computers { get; set; } = new List<Computer>();

    public virtual ICollection<LabUsageLog> LabUsageLogs { get; set; } = new List<LabUsageLog>();

    public virtual ICollection<MaintenanceRequest> MaintenanceRequests { get; set; } = new List<MaintenanceRequest>();

    public virtual ICollection<Software> Softwares { get; set; } = new List<Software>();
}
