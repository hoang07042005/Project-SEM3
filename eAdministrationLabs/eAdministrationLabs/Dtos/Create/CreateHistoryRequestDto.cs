namespace eAdministrationLabs.Dtos.Create
{
    public class CreateHistoryRequestDto
    {

        public int UserId { get; set; }

        public int RequestId { get; set; }

        public int StatusRequestId { get; set; }

        public string ChangedBy { get; set; } = null!;

        public string? Notes { get; set; }
    }
}
