using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagement.Models.Entities
{
    [Table("Documents")]
    public partial class Document
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public string? Content { get; set; }
        public int? SenderId { get; set; }
        public string? SentFrom { get; set; }
        public int? UrgencyLevelId { get; set; }
        public int? StatusId { get; set; }
        public DateTime? CreatedAt { get; set; }

        [ForeignKey("SenderId")]
        public virtual User? Sender { get; set; }
        [ForeignKey("StatusId")]
        public virtual DocumentStatus? Status { get; set; }
        [ForeignKey("UrgencyLevelId")]
        public virtual UrgencyLevel? UrgencyLevel { get; set; }

        public virtual ICollection<DocumentAttachment> DocumentAttachments { get; set; } = new List<DocumentAttachment>();
        public virtual ICollection<DocumentRecipient> DocumentRecipients { get; set; } = new List<DocumentRecipient>();
        public virtual ICollection<DocumentLog> DocumentLogs { get; set; } = new List<DocumentLog>();
    }
}
