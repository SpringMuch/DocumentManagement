using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentManagement.Models.Entities
{
    [Table("Users")]
    public partial class User
    {
        [Key]
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string? Workplace { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
