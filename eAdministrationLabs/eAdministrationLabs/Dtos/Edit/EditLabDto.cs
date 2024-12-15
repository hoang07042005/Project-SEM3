namespace eAdministrationLabs.Dtos.Edit
{
    public class EditLabDto
    {
        public int Id { get; set; }

        public string LabName { get; set; } = null!;

        public string Location { get; set; } = null!;

        public int Capacity { get; set; }

        public int StatusLabId { get; set; }

        public DateTime? CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
