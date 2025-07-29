using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagement.Models.Entities
{
    [Table("UrgencyLevels")]
    public partial class UrgencyLevel
    {
        [Key]
        public int Id { get; set; }
        public string LevelName { get; set; }
    }
}
