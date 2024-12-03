namespace eAdministrationLabs.Dtos.Edit
{
    public class ComputerEditDto
    {
        public int Id { get; set; }
        public int? LabId { get; set; }
        public string AssetTag { get; set; } = null!;
        public string? Status { get; set; }
        public string? Specifications { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
