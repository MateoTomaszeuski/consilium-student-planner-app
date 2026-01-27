using Consilium.Shared.Models;
using System.Net.Http.Json;

namespace Consilium.Shared.Services;

public class AssignmentService(IClientService clientService) : IAssignmentService {

    public List<Assignment> AllAssignments { get; set; } = new();

    public async Task AddAssignmentAsync(Assignment a) {
        var response = await clientService.PostAsync("assignment", a);

        if (!response.IsSuccessStatusCode) {
            throw new Exception("Failed to add assignment.");
        }
    }

    public async Task DeleteAssignmentAsync(int assignmentId) {
        var response = await clientService.DeleteAsync($"assignment?assignmentId={assignmentId}");
    }

    public async Task<IEnumerable<Assignment>> GetAllAssignmentsAsync() {
        var response = await clientService.GetAsync("assignment");
        if (!response.IsSuccessStatusCode) {
            throw new Exception("Failed to fetch assignments");
        }

        var assignments = await response.Content.ReadFromJsonAsync<IEnumerable<Assignment>>();
        if (assignments is null) {
            throw new Exception("Failed to deserialize assignments");
        }

        return assignments;
    }

    public async Task UpdateAssignmentAsync(Assignment a) {
        await clientService.PatchAsync("assignment", a);
    }

    public async Task<IEnumerable<Course>> GetAllCoursesAsync() {
        var response = await clientService.GetAsync("assignment/courses");
        if (!response.IsSuccessStatusCode) {
            throw new Exception("Failed to fetch courses");
        }

        var courses = await response.Content.ReadFromJsonAsync<IEnumerable<Course>>();
        if (courses is null) {
            throw new Exception("Failed to deserialize courses");
        }
        return courses;
    }

    public async Task AddCourseAsync(Course c) {
        var response = await clientService.PostAsync("assignment/course", c);
        if (!response.IsSuccessStatusCode) {
            throw new Exception("Failed to add course");
        }
    }

    public async Task DeleteCourseAsync(int courseId) {
        var response = await clientService.DeleteAsync($"assignment/course?courseid={courseId}");

        if (!response.IsSuccessStatusCode) {
            throw new Exception("Failed to delete course");
        }
    }

}