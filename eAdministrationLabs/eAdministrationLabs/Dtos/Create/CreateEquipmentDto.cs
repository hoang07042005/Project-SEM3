namespace eAdministrationLabs.Dtos.Create
{
    public class CreateEquipmentDto
    {
    

        public string NameEquipment { get; set; } = null!;

        public string Type { get; set; } = null!;

        public DateTime? PurchaseDate { get; set; }
    }
}
