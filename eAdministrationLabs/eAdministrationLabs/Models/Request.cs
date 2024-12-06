using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class Request
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int LabId { get; set; }

    public int? EquipmentId { get; set; }

    public int ImageId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Equipment? Equipment { get; set; }

    public virtual ICollection<HistoryRequest> HistoryRequests { get; set; } = new List<HistoryRequest>();

    public virtual RequestImage Image { get; set; } = null!;

    public virtual Lab Lab { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
