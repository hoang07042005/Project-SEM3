using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<EquiLab> EquiLabs { get; set; } = new List<EquiLab>();

    public virtual ICollection<LabUsageLog> LabUsageLogs { get; set; } = new List<LabUsageLog>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
