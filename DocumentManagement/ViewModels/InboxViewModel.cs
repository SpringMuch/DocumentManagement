using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentManagement.Models.Entities;

namespace DocumentManagement.ViewModels
{
    public class InboxViewModel
    {
        public List<Document> Documents { get; set; }
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public IEnumerable<SelectListItem> UrgencyLevelList { get; set; }
        public string? Keyword { get; set; }
        public int? StatusId { get; set; }
        public int? UrgencyLevelId { get; set; }
        public int TotalCount { get; set; }
        public int UnprocessedCount { get; set; }
        public int InProgressCount { get; set; }
        public int ProcessedCount { get; set; }
        public int PageIndex { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
    }
}