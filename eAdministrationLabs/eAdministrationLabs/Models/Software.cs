using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class Software
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string Version { get; set; } = null!;

    public string? License { get; set; }

    public int LabId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual Lab Lab { get; set; } = null!;
}
