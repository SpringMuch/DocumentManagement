using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentManagement.Models.Entities;
namespace DocumentManagement.ViewModels
{
    public class SentViewModel
    {
        public IEnumerable<Document> Documents { get; set; }

        public string? Keyword { get; set; }
        public int? StatusId { get; set; }

        public IEnumerable<SelectListItem> StatusList { get; set; }
        public IEnumerable<SelectListItem> RecipientList { get; set; }
 
        public int TotalCount { get; set; }
        public int SentCount { get; set; }
        public int InProgressCount { get; set; }
        public int RespondedCount { get; set; }
    }
}
