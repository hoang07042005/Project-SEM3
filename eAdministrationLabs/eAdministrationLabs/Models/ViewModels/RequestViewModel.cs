namespace eAdministrationLabs.Models.ViewModels
{
    public class RequestViewModel
    {
        public int Id { get; set; }
        public string LabName { get; set; } = string.Empty;
        public string? EquipmentName { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string ChangedBy { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? ImageBase64 { get; set; } // Hiển thị ảnh dưới dạng base64
    }
}
