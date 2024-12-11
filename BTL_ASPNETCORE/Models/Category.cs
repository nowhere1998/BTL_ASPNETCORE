using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_ASPNETCORE.Models
{
    [Table("Categories")]
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; }
        [Required]
        [StringLength(100)]
        public string? CategoryName { get; set; }
        public bool Status { get; set; } = true;
        public ICollection<Dish> Dishes { get; set; } = new HashSet<Dish>();
    }
}
