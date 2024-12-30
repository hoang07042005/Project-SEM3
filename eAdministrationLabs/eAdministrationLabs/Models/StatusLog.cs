namespace eAdministrationLabs.Models
{
    public class StatusLog
    {

        public int Id { get; set; }

        public string StatusName { get; set; } = null!;

        public virtual ICollection<LabUsageLog> LabUsageLogs { get; set; } = new List<LabUsageLog>();
        
    }
}
