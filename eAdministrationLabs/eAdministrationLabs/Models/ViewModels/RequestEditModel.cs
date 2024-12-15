using System;
using System.ComponentModel.DataAnnotations;

namespace eAdministrationLabs.Models.ViewModels
{
    public class RequestEditModel
    {
        public int Id { get; set; } // ID của yêu cầu

        [Required]
        public int LabId { get; set; } // ID của Lab

        [Required]
        public int? EquipmentId { get; set; } // ID của Equipment

        [Required]
        public int? StatusRequestId { get; set; } // ID của StatusRequest

        public string Notes { get; set; } // Ghi chú

        public string ImageBase64 { get; set; } // Dữ liệu ảnh dạng base64 để hiển thị ảnh hiện tại

        // Bạn có thể thêm các thuộc tính bổ sung nếu cần
    }
}
