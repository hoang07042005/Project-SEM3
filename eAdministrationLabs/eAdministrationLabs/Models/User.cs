using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace eAdministrationLabs.Models;

public partial class User
{
    public int Id { get; set; }

    public string FullName { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public string? PasswordResetToken { get; set; } 

    public DateTime? TokenExpirationTime { get; set; }

    public virtual ICollection<HistoryRequest> HistoryRequests { get; set; } = new List<HistoryRequest>();

    [JsonIgnore]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    public virtual ICollection<LabUsageLog> LabUsageLogs { get; set; } = new List<LabUsageLog>();

    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
