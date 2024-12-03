namespace eAdministrationLabs.Dtos.Edit
{
    public class NotificationEditDto
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string Message { get; set; } = null!;
        public DateTime? CreatedAt { get; set; }
    }
}
