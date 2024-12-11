using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BTL_ASPNETCORE.Models
{
    [Table("Dishes")]
    public class Dish
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DishId { get; set; }
        [Required]
        [StringLength(100)]
        public string? DishName { get; set; }
        [Required]
        [Range(1, int.MaxValue)]
        public float Price { get; set; }
        [Range(0, int.MaxValue)]
        public float SalePrice { get; set; } = 0;
        public int Size { get; set; } = 0;
        public string? Image {  get; set; }
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category? Category { get; set; }
    }
}
