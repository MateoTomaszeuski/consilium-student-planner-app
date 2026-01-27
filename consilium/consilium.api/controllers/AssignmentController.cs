using Consilium.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Consilium.API.Controllers;

[ApiController]
[Route("[controller]")]
public class AssignmentController : ControllerBase {

    private readonly IDBService service;
    private readonly ILogger<AssignmentController> logger;

    public AssignmentController(IDBService service, ILogger<AssignmentController> logger) {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet]
    public IEnumerable<Assignment> GetAllAssignments() {
        string username = Request.Headers["Email-Auth_Email"]!;
        logger.LogInformation("Getting all assignments for {username}", username);
        return service.GetAllAssignments(username);
    }

    [HttpPost]
    public IResult PostAssignment(Assignment assignment) {
        string username = Request.Headers["Email-Auth_Email"]!;
        int value = service.AddAssignment(assignment, username);
        logger.LogInformation("Adding assignment for {username}", username);
        if (value == -1) {
            return Results.Unauthorized();
        } else {
            return Results.Ok(value);
        }
    }

    [HttpDelete]
    public IResult DeleteAssignment(int assignmentId) {
        string username = Request.Headers["Email-Auth_Email"]!;
        service.DeleteAssignment(assignmentId, username);
        logger.LogInformation("Deleting assignment for {username}", username);
        return Results.Ok();
    }

    [HttpGet("incomplete")]
    public IEnumerable<Assignment> GetIncompleteAssignments() {
        string username = Request.Headers["Email-Auth_Email"]!;
        logger.LogInformation("Getting incomplete assignments for {username}", username);
        return service.GetIncompleteAssignments(username);
    }

    [HttpPatch]
    public IResult UpdateAssignment(Assignment a) {
        string username = Request.Headers["Email-Auth_Email"]!;
        service.UpdateAssignment(a, username);
        logger.LogInformation("Updating assignment for {username}", username);
        return Results.Ok();
    }

    [HttpGet("courses")]
    public IEnumerable<Course> GetAllCourses() {
        string username = Request.Headers["Email-Auth_Email"]!;
        logger.LogInformation("Getting all courses for {username}", username);
        return service.GetAllCourses(username);

    }

    [HttpPost("course")]
    public IResult PostCourse(Course course) {
        string username = Request.Headers["Email-Auth_Email"]!;
        int value = service.AddCourse(course, username);
        logger.LogInformation("Adding course for {username}", username);
        if (value == -1) {
            return Results.Unauthorized();
        } else {
            return Results.Ok(value);
        }
    }

    [HttpDelete("course")]
    public IResult DeleteCourse(int courseId) {
        string username = Request.Headers["Email-Auth_Email"]!;
        service.DeleteCourse(courseId, username);
        logger.LogInformation("Deleting course for {username}", username);
        return Results.Ok();
    }

}