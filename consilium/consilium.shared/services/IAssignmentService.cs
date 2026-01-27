using Consilium.Shared.Models;

namespace Consilium.Shared.Services;

public interface IAssignmentService {
    List<Assignment> AllAssignments { get; set; }
    Task AddAssignmentAsync(Assignment a);
    Task DeleteAssignmentAsync(int assignmentId);
    Task<IEnumerable<Assignment>> GetAllAssignmentsAsync();
    Task UpdateAssignmentAsync(Assignment a);
    Task<IEnumerable<Course>> GetAllCoursesAsync();
    Task AddCourseAsync(Course c);
    Task DeleteCourseAsync(int courseId);
}