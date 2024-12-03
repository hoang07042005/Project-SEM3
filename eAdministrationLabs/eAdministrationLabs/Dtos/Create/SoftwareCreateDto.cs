namespace eAdministrationLabs.Dtos.Create
{
    public class SoftwareCreateDto
    {
        public string Name { get; set; } = null!;
        public string Version { get; set; } = null!;
        public int? LabId { get; set; }
    }
}
