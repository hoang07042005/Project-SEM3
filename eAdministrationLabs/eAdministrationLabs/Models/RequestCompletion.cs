    namespace eAdministrationLabs.Models
    {
        public class RequestCompletion
        {
            public int Id { get; set; }
            public int HistoryRequestId { get; set; } 
            public DateTime CompletionTime { get; set; }  
            public string CompletedBy { get; set; }  
            public string ImageBase64 { get; set; }  

            public virtual HistoryRequest HistoryRequest { get; set; }

        }

    }
