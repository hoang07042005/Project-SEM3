namespace eAdministrationLabs.Dtos.Create
{
    public class ComputerCreateDto
    {
        public int? LabId { get; set; }
        public string AssetTag { get; set; } = null!;
        public string? Status { get; set; }
        public string? Specifications { get; set; }
    }
}
