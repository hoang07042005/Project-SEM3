using System.ComponentModel.DataAnnotations;

namespace eAdministrationLabs.Models
{
    public class Feedback
    {
        public int Id { get; set; }

        public int RequestId { get; set; } 

        public int UserId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }  

        [StringLength(500, ErrorMessage = "Comment cannot be longer than 500 characters.")]
        public string? Comment { get; set; } 

        [Range(1, 5, ErrorMessage = "Satisfaction must be between 1 and 5.")]
        public int Satisfaction { get; set; } 

        [Range(1, 5, ErrorMessage = "Quality must be between 1 and 5.")]
        public int Quality { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } 

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }

        public virtual HistoryRequest? HistoryRequest { get; set; } = null!;

        public virtual User? User { get; set; } = null!;
    }

}
