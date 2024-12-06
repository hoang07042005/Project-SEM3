using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class HistoryRequest
{
    public int Id { get; set; }

    public int UserId { get; set; }

    public int RequestId { get; set; }

    public int StatusRequestId { get; set; }

    public string ChangedBy { get; set; } = null!;

    public DateTime? ChangedAt { get; set; }

    public string? Notes { get; set; }

    public virtual Request Request { get; set; } = null!;

    public virtual StatusRequest StatusRequest { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
