using Consilium.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Consilium.API.Controllers;

[ApiController]
[Route("[controller]")]
public class todoController : ControllerBase {

    private readonly IDBService service;
    private readonly ILogger<todoController> logger;

    public todoController(IDBService service, ILogger<todoController> logger) {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet(Name = "GetTodos")]
    public IEnumerable<TodoItem> Get() {
        string username = Request.Headers["Email-Auth_Email"]!; // Cody - I know this will be there at this point
        logger.LogInformation("Getting all todos for {username}", username);
        return service.GetTodoList(username);
    }

    [HttpPatch("update", Name = "PatchTodos")]
    public IResult Update(TodoItem item) {
        string username = Request.Headers["Email-Auth_Email"]!;
        logger.LogInformation("Updating todo for {username}", username);
        try {
            service.UpdateToDo(item, username);
        } catch (Exception e) {
            logger.LogError("Error updating todo for {username}: {error}", username, e.Message);
            return Results.BadRequest(e.Message);
        }
        return Results.Accepted();
    }


    [HttpPost(Name = "CreateTodos")]
    public IResult Post(TodoItem item) {
        string username = Request.Headers["Email-Auth_Email"]!;
        int result = service.AddToDo(item, username);
        logger.LogInformation("Adding todo for {username}", username);
        return Results.Ok(result);
    }

    [HttpDelete("remove/{item}", Name = "RemoveTodos")]
    public IResult Remove(int item) {
        try {
            string username = Request.Headers["Email-Auth_Email"]!;
            service.RemoveToDo(item, username);
        } catch (Exception e) {
            return Results.BadRequest(e.Message);
        }
        logger.LogInformation("Removing todo for {username}", Request.Headers["Email-Auth_Email"]!);
        return Results.Accepted();
    }
}