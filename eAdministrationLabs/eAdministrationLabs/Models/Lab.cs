using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class Lab
{
    public int Id { get; set; }

    public string LabName { get; set; } = null!;

    public string Location { get; set; } = null!;

    public int Capacity { get; set; }

    public int StatusLabId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<EquiLab> EquiLabs { get; set; } = new List<EquiLab>();

    public virtual ICollection<LabUsageLog> LabUsageLogs { get; set; } = new List<LabUsageLog>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<Software> Softwares { get; set; } = new List<Software>();

    public virtual StatusLab StatusLab { get; set; } = null!;
}
