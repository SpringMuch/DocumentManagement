using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagement.Models.Entities
{
    [Table("DocumentAttachments")]
    public partial class DocumentAttachment
    {
        [Key]
        public int Id { get; set; }
        public int DocumentId { get; set; }
        public string FileName { get; set; }
        public string? FilePath { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document Document { get; set; }
    }
}
