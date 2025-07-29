using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagement.Models.Entities
{
    [Table("DocumentRecipients")]
    public partial class DocumentRecipient
    {
        [Key]
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public int RecipientId { get; set; }
        public bool? IsMarked { get; set; }
        public bool? IsDeleted { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }
        [ForeignKey("RecipientId")]
        public virtual User Recipient { get; set; }
    }
}
