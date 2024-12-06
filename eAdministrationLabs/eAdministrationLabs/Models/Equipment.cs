using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class Equipment
{
    public int Id { get; set; }

    public string NameEquipment { get; set; } = null!;

    public string Type { get; set; } = null!;

    public DateTime? PurchaseDate { get; set; }

    public virtual ICollection<EquiLab> EquiLabs { get; set; } = new List<EquiLab>();

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
