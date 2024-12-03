namespace eAdministrationLabs.Dtos.Edit
{
    public class MaintenanceRequestEditDto
    {
        public int Id { get; set; }
        public int? LabId { get; set; }
        public int? ComputerId { get; set; }
        public string Description { get; set; } = null!;
        public string? Status { get; set; }
        public int? RequestedBy { get; set; }
    }
}
