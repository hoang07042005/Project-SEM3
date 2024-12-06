using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class StatusLab
{
    public int Id { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<Lab> Labs { get; set; } = new List<Lab>();
}
