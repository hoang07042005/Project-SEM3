namespace eAdministrationLabs.Dtos.Create
{
    public class CreateLabUsageLogDto
    {
        public int LabId { get; set; }

        public int UserId { get; set; }

        public string Purpose { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

    }
}
