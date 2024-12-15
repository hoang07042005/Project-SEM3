namespace eAdministrationLabs.Dtos.Create
{
    public class CreateLabDto
    {

        public string LabName { get; set; } = null!;

        public string Location { get; set; } = null!;

        public int Capacity { get; set; }

        public int StatusLabId { get; set; }
    }
}
