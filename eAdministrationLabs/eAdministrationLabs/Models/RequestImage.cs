using System;
using System.Collections.Generic;

namespace eAdministrationLabs.Models;

public partial class RequestImage
{
    public int Id { get; set; }

    public byte[] Image { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Request> Requests { get; set; } = new List<Request>();
}
