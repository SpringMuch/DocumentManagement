using DocumentManagement.Models.Entities;

namespace DocumentManagement.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalIncoming { get; set; }
        public int UnreadIncoming { get; set; }
        public int TotalSent { get; set; }
        public IEnumerable<Document> RecentDocuments { get; set; }
    }
}