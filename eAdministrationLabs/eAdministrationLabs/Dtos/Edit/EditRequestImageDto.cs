namespace eAdministrationLabs.Dtos.Edit
{
    public class EditRequestImageDto
    {
        public int Id { get; set; }

        public IFormFile? Image { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
