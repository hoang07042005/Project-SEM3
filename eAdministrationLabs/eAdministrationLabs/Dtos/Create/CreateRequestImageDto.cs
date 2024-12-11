namespace eAdministrationLabs.Dtos.Create
{
    public class CreateRequestImageDto
    {

        public IFormFile Image { get; set; } 

        public DateTime? CreatedAt { get; set; }

    }
}
