using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_ASPNETCORE.Models
{
    [Table("Chefs")]
    public class Chef
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChefId { get; set; }
        [Required]
        [StringLength(100)]
        public string? ChefName { get; set; }
        public string? Image {  get; set; }
        [Column(TypeName = "text")]
        public string? Description { get; set; }
    }
}
