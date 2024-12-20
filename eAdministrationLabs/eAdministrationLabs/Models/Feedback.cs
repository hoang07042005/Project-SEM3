using System.ComponentModel.DataAnnotations;

namespace eAdministrationLabs.Models
{
    public class Feedback
    {
        public int Id { get; set; }              // Feedback identifier

        public int RequestId { get; set; }       // Linked request ID (foreign key from HistoryRequests)

        public int UserId { get; set; }

        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }          // Rating (1 to 5)

        [StringLength(500, ErrorMessage = "Comment cannot be longer than 500 characters.")]
        public string? Comment { get; set; }     // User's feedback comment

        [Range(1, 5, ErrorMessage = "Satisfaction must be between 1 and 5.")]
        public int Satisfaction { get; set; }    // Satisfaction level (1 to 5)

        [Range(1, 5, ErrorMessage = "Quality must be between 1 and 5.")]
        public int Quality { get; set; }         // Quality level (1 to 5)

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; }  // Feedback creation time

        [DataType(DataType.DateTime)]
        public DateTime UpdatedAt { get; set; }  // Feedback update time

        // This is optional, not required for submission
        public virtual HistoryRequest? HistoryRequest { get; set; } = null!;

        public virtual User? User { get; set; } = null!;
    }

}
