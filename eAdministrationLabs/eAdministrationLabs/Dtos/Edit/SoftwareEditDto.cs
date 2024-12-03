namespace eAdministrationLabs.Dtos.Edit
{
    public class SoftwareEditDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Version { get; set; } = null!;
        public int? LabId { get; set; }
    }
}
