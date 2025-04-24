using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ContosoUniversity.Models.ViewModels
{
    public class CourseViewModel
    {
        public int CourseID { get; set; }
        
        [Required]
        [StringLength(50, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        
        [Range(0, 5)]
        public int Credits { get; set; }
        
        [Required]
        public int DepartmentID { get; set; }
        
        public IEnumerable<SelectListItem>? Departments { get; set; }
        public IEnumerable<SelectListItem>? Instructors { get; set; }
        public List<int> SelectedInstructors { get; set; } = new List<int>();
    }
} 