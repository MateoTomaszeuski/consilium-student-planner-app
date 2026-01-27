using Consilium.Shared.Models;
using Microsoft.AspNetCore.Mvc;

namespace Consilium.API.Controllers;

[ApiController]
[Route("[controller]")]
public class NewFeatureController : ControllerBase {
    private readonly ILogger<NewFeatureController> logger;
    private HttpClient client;

    public NewFeatureController(IHttpClientFactory factory, ILogger<NewFeatureController> logger) {
        this.logger = logger;
        client = factory.CreateClient("FeedbackWebhock");
    }
    [HttpGet]
    public string GetNewFeature() {
        logger.LogInformation("New featrure clicked");
        return "Received";
    }
    
    [HttpPost("feedback")]
    public async Task<IResult> PostFeedback([FromBody] FeedbackRequest request) {
        logger.LogInformation("Feedback received");

        if (string.IsNullOrWhiteSpace(request.Feedback)) {
            return Results.BadRequest("Feedback cannot be empty");
        }

        var payload = new { content = request.Feedback };

        try {
            var response = await client.PostAsJsonAsync("", payload);

            if (!response.IsSuccessStatusCode) {
                logger.LogError("Discord webhook failed with {StatusCode}", response.StatusCode);
                return Results.StatusCode(500);
            }

            return Results.Ok("Received");
        } catch (Exception e) {
            logger.LogError("Error sending feedback: {Error}", e.Message);
            return Results.StatusCode(500);
        }
    }
}

public class FeedbackRequest {
    public string Feedback { get; set; } = string.Empty;
}