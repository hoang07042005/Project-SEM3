using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class StatusRequest
{
    public int Id { get; set; }

    public string StatusName { get; set; } = null!;

    public virtual ICollection<HistoryRequest> HistoryRequests { get; set; } = new List<HistoryRequest>();
}
