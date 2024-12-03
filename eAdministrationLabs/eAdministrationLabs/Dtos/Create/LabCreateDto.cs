namespace eAdministrationLabs.Dtos.Create
{
    public class LabCreateDto
    {
        public string LabName { get; set; } = null!;
        public string Location { get; set; } = null!;
        public int Capacity { get; set; }
        public string? Status { get; set; }
    }
}
