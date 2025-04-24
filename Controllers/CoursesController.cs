using ContosoUniversity.Data;
using ContosoUniversity.Models;
using ContosoUniversity.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContosoUniversity.Controllers
{
    public class CoursesController : Controller
    {
        private readonly SchoolContext _context;

        public CoursesController(SchoolContext context)
        {
            _context = context;
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .OrderBy(c => c.CourseID)
                .ToListAsync();
                
            return View(courses);
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.CourseAssignments)
                    .ThenInclude(ca => ca.Instructor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
                
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // GET: Courses/Create
        public IActionResult Create()
        {
            var course = new CourseViewModel();
            PopulateDepartmentsDropDownList();
            PopulateInstructorsDropDownList();
            return View(course);
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseViewModel courseViewModel)
        {
            if (ModelState.IsValid)
            {
                var course = new Course
                {
                    CourseID = courseViewModel.CourseID,
                    Title = courseViewModel.Title,
                    Credits = courseViewModel.Credits,
                    DepartmentID = courseViewModel.DepartmentID
                };
                
                _context.Add(course);
                await _context.SaveChangesAsync();

                // Add selected instructors to the course
                if (courseViewModel.SelectedInstructors != null)
                {
                    foreach (var instructorId in courseViewModel.SelectedInstructors)
                    {
                        _context.CourseAssignments.Add(new CourseAssignment
                        {
                            CourseID = course.CourseID,
                            InstructorID = instructorId
                        });
                    }
                    await _context.SaveChangesAsync();
                }
                
                return RedirectToAction(nameof(Index));
            }
            
            PopulateDepartmentsDropDownList(courseViewModel.DepartmentID);
            PopulateInstructorsDropDownList(courseViewModel.SelectedInstructors);
            return View(courseViewModel);
        }

        // GET: Courses/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.CourseAssignments).ThenInclude(ca => ca.Instructor)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
                
            if (course == null)
            {
                return NotFound();
            }

            var viewModel = new CourseViewModel
            {
                CourseID = course.CourseID,
                Title = course.Title,
                Credits = course.Credits,
                DepartmentID = course.DepartmentID,
                SelectedInstructors = course.CourseAssignments.Select(ca => ca.InstructorID).ToList()
            };
            
            PopulateDepartmentsDropDownList(course.DepartmentID);
            PopulateInstructorsDropDownList(viewModel.SelectedInstructors);
            return View(viewModel);
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CourseViewModel courseViewModel)
        {
            if (id != courseViewModel.CourseID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var courseToUpdate = await _context.Courses
                        .Include(c => c.CourseAssignments)
                        .FirstOrDefaultAsync(c => c.CourseID == id);

                    if (courseToUpdate == null)
                    {
                        return NotFound();
                    }

                    courseToUpdate.CourseID = courseViewModel.CourseID;
                    courseToUpdate.Title = courseViewModel.Title;
                    courseToUpdate.Credits = courseViewModel.Credits;
                    courseToUpdate.DepartmentID = courseViewModel.DepartmentID;

                    // Update course assignments
                    UpdateCourseInstructors(courseViewModel.SelectedInstructors, courseToUpdate);
                    
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateException)
                {
                    ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists, see your system administrator.");
                }
            }
            
            PopulateDepartmentsDropDownList(courseViewModel.DepartmentID);
            PopulateInstructorsDropDownList(courseViewModel.SelectedInstructors);
            return View(courseViewModel);
        }

        // GET: Courses/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var course = await _context.Courses
                .Include(c => c.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CourseID == id);
                
            if (course == null)
            {
                return NotFound();
            }

            return View(course);
        }

        // POST: Courses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            
            return RedirectToAction(nameof(Index));
        }

        private void PopulateDepartmentsDropDownList(object selectedDepartment = null)
        {
            var departmentsQuery = from d in _context.Departments
                                   orderby d.Name
                                   select d;
                                   
            ViewBag.Departments = new SelectList(departmentsQuery.AsNoTracking(),
                "DepartmentID", "Name", selectedDepartment);
        }

        private void PopulateInstructorsDropDownList(List<int> selectedInstructors = null)
        {
            var instructorsQuery = from i in _context.Instructors
                                   orderby i.LastName
                                   select i;
                                   
            ViewBag.Instructors = new SelectList(instructorsQuery.AsNoTracking(),
                "ID", "FullName");
                
            ViewBag.SelectedInstructors = selectedInstructors;
        }

        private void UpdateCourseInstructors(List<int> selectedInstructors, Course courseToUpdate)
        {
            if (selectedInstructors == null)
            {
                courseToUpdate.CourseAssignments = new List<CourseAssignment>();
                return;
            }

            var selectedInstructorsHS = new HashSet<int>(selectedInstructors);
            var courseInstructorsHS = new HashSet<int>(
                courseToUpdate.CourseAssignments.Select(c => c.InstructorID));
                
            foreach (var instructor in _context.Instructors)
            {
                if (selectedInstructorsHS.Contains(instructor.ID))
                {
                    if (!courseInstructorsHS.Contains(instructor.ID))
                    {
                        courseToUpdate.CourseAssignments.Add(
                            new CourseAssignment { CourseID = courseToUpdate.CourseID, InstructorID = instructor.ID });
                    }
                }
                else
                {
                    if (courseInstructorsHS.Contains(instructor.ID))
                    {
                        CourseAssignment courseAssignmentToRemove = courseToUpdate.CourseAssignments
                            .FirstOrDefault(i => i.InstructorID == instructor.ID);
                        _context.Remove(courseAssignmentToRemove);
                    }
                }
            }
        }
    }
} 