using eAdministrationLabs.Dtos.Create;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eAdministrationLabs.Models.ViewModels
{
    public class CreateLabUsageLogViewModel
    {
        public SelectList Labs { get; set; }
        public SelectList Users { get; set; }
        public CreateLabUsageLogDto CreateLabUsageLogDto { get; set; }
    }
}
