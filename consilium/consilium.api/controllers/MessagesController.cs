using Consilium.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Consilium.API.Controllers;

[ApiController]
[Route("[controller]")]
public class MessagesController : ControllerBase {

    private readonly IDBService service;
    private readonly ILogger<MessagesController> logger;

    public MessagesController(IDBService service, ILogger<MessagesController> logger) {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet("all")]
    public IEnumerable<string> GetAllConversations() {
        string username = Request.Headers["Email-Auth_Email"]!;
        logger.LogInformation("Getting all conversations for {username}", username);
        return service.GetConversations(username);
    }
    [HttpGet("{otherUser}")]
    public IEnumerable<Message> GetAllMessages(string otherUser) {
        string username = Request.Headers["Email-Auth_Email"]!;
        logger.LogInformation("Getting all messages for {username} and {otherUser}", username, otherUser);
        return service.GetMessages(username, otherUser);
    }
    [HttpGet("/check/{otherUser}")]
    public bool GetUser(string otherUser) {
        string username = Request.Headers["Email-Auth_Email"]!;
        logger.LogInformation("Checking if {otherUser} is a user for {username}", otherUser, username);
        return service.CheckUser(otherUser);
    }

    [HttpPost]
    public async Task<string> PostNewMessage(Message message) {
        string username = Request.Headers["Email-Auth_Email"]!;
        message.Sender = username;
        logger.LogInformation("Sending message from {username} to {otherUser}", username, message.Receiver);
        return await service.AddMessage(message);
    }

}