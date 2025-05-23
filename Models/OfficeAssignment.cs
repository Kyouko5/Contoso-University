using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ContosoUniversity.Models
{
    public class OfficeAssignment
    {
        [Key]
        public int InstructorID { get; set; }
        
        [StringLength(50)]
        [Display(Name = "Office Location")]
        public string Location { get; set; } = string.Empty;
        
        public Instructor? Instructor { get; set; }
    }
} 