using Microsoft.AspNetCore.Mvc.Rendering;
using DocumentManagement.Models.Entities;

namespace DocumentManagement.ViewModels
{
    public class SearchViewModel
    {
        // === Tiêu chí tìm kiếm (dữ liệu vào) ===
        public string? Keyword { get; set; }
        public int? StatusId { get; set; }
        public int? UrgencyLevelId { get; set; }
        public int? SenderId { get; set; } // Tìm theo người gửi
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        // public string? DocumentNumber { get; set; } // Có thể thêm nếu Model Document có SoKyHieu

        // === Dữ liệu trả về ===
        public IEnumerable<Document> SearchResults { get; set; }
        public int? ResultCount { get; set; } // Để hiển thị "Tìm thấy x kết quả"

        // === Dữ liệu để điền vào các dropdown ===
        public IEnumerable<SelectListItem> StatusList { get; set; }
        public IEnumerable<SelectListItem> UrgencyLevelList { get; set; }
        public IEnumerable<SelectListItem> SenderList { get; set; } // Dùng cho dropdown "Đơn vị"
    }
}