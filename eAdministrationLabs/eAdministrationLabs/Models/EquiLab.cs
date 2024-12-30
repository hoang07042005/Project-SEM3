using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class EquiLab
{
    public int Id { get; set; }

    public int EquipmentId { get; set; }

    public int LabId { get; set; }

    public virtual Equipment Equipment { get; set; } = null!;

    public virtual Lab Lab { get; set; } = null!;

}
