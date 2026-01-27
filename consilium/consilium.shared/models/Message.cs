namespace Consilium.Shared.Models;
public class Message {
    public string Sender { get; set; } = string.Empty;
    public string Receiver { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public DateTime TimeSent { get; set; }
    public bool? IsMyMessage { get; set; }
}