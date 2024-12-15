namespace eAdministrationLabs.Dtos.Edit
{
    public class EditRequestDto
    {
        public int Id { get; set; }

        public int LabId { get; set; }

        public int? EquipmentId { get; set; }

        public int ImageId { get; set; }

        public DateTime? CreatedAt { get; set; }
    }
}
