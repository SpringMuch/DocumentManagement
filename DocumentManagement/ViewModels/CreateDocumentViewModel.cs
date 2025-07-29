using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentManagement.Models.Entities;

namespace DocumentManagement.ViewModels
{
    public class CreateDocumentViewModel
    {
        public Document Document { get; set; } = new Document();
        public List<int> RecipientIds { get; set; } = new List<int>();

        // Dữ liệu để điền vào các dropdown
        public IEnumerable<SelectListItem> RecipientList { get; set; }
        public IEnumerable<SelectListItem> UrgencyLevelList { get; set; } // Thêm nếu cần
        public IEnumerable<SelectListItem> StatusList { get; set; } // Thêm nếu cần

        // Dữ liệu cho phần "Văn bản gần đây"
        public IEnumerable<Document> RecentDocuments { get; set; }
    }
}