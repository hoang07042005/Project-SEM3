namespace eAdministrationLabs.Dtos.Edit
{
    public class EditHistoryRequestDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int RequestId { get; set; }

        public int StatusRequestId { get; set; }

        public string ChangedBy { get; set; } = null!;

        public DateTime? ChangedAt { get; set; }

        public string? Notes { get; set; }
    }
}
