namespace eAdministrationLabs.Dtos.Create
{
    public class MaintenanceRequestCreateDto
    {
        public int? LabId { get; set; }
        public int? ComputerId { get; set; }
        public string Description { get; set; } = null!;
        public string? Status { get; set; }
        public int? RequestedBy { get; set; }
        public DateTime? ResolvedAt { get; set; }
    }
}
