using Consilium.Shared.Models;
using System.ComponentModel;

namespace Consilium.Shared.Services;
public interface IMessageService {
    public string CurrentChat { get; set; }
    event PropertyChangedEventHandler? PropertyChanged;
    Task<bool> CheckUser(string newConversationName);
    Task<IEnumerable<string>> GetConversations();
    Task<IEnumerable<Message>> InitializeMessagesAsync(string otherUser);
    Task<bool> SendMessageAsync(Message message);
}