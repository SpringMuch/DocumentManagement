using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagement.Models.Entities
{
    [Table("DocumentStatus")]
    public partial class DocumentStatus
    {
        [Key]
        public int Id { get; set; }
        public string StatusName { get; set; }
    }
}
