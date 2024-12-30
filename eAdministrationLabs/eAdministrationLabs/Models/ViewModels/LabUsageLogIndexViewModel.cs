namespace eAdministrationLabs.Models.ViewModels
{
    public class LabUsageLogIndexViewModel
    {
        public List<LabUsageLog> PendingLogs { get; set; }
        public List<LabUsageLog> ApprovedLogs { get; set; }
    }

}
