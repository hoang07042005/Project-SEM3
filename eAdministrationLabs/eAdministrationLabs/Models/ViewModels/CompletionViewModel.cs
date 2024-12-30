namespace eAdministrationLabs.Models.ViewModels
{
    public class CompletionViewModel
    {
        public string LabName { get; set; }
        public string NameEquipment { get; set; }
        public DateTime CompletionTime { get; set; }
        public string CompletedBy { get; set; }
        public int HistoryRequestId { get; set; }
        public IFormFile ImageFile { get; set; }  // For the image upload
    }
}
