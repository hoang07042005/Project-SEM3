using System.ComponentModel.DataAnnotations;

namespace eAdministrationLabs.Models.ViewModels
{
    public class RequestCreateModel
    {
        [Required]
        public int LabId { get; set; }

        public int? EquipmentId { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public int StatusRequestId { get; set; }

        [Required]
        public string ChangedBy { get; set; } = null!;

        public string? Notes { get; set; }

        [Required]
        public IFormFile Image { get; set; } = null!;
    }
}
