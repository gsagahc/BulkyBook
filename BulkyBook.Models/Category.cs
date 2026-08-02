using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BulkyBook.Models
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;
        [Range (0,100, ErrorMessage = "Display order must be between 0 and 100")]
        public int DisplayOrder { get; set; } 
        
        public int OrderID { get; set; }
    }
}