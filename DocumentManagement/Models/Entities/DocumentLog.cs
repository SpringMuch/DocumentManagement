using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagement.Models.Entities
{
    [Table("DocumentLogs")]
    public partial class DocumentLog
    {
        [Key]
        public int Id { get; set; }
        public int? DocumentId { get; set; }
        public string? Action { get; set; }
        public int? PerformedBy { get; set; }
        public DateTime? PerformedAt { get; set; }

        [ForeignKey("DocumentId")]
        public virtual Document? Document { get; set; }
        [ForeignKey("PerformedBy")]
        public virtual User? Performer { get; set; }
    }
}
