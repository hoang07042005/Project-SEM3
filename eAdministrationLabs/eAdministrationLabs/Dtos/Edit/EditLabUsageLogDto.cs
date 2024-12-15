namespace eAdministrationLabs.Dtos.Edit
{
    public class EditLabUsageLogDto
    {
        public int Id { get; set; }

        public int LabId { get; set; }

        public int UserId { get; set; }

        public string Purpose { get; set; } = null!;

        public DateTime StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
