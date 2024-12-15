namespace eAdministrationLabs.Dtos.Edit
{
    public class EditEquipmentDto
    {
        public int Id { get; set; }

        public string NameEquipment { get; set; } = null!;

        public string Type { get; set; } = null!;

        public DateTime? PurchaseDate { get; set; }
    }
}
