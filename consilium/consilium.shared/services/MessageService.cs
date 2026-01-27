using Consilium.Shared.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Http.Json;

namespace Consilium.Shared.Services;

public class MessageService : IMessageService, INotifyPropertyChanged {
    private readonly IClientService client;

    public MessageService(IClientService client) {
        this.client = client;
    }

    private string _currentChat = string.Empty;

    public string CurrentChat {
        get => _currentChat;
        set {
            if (_currentChat != value) {
                _currentChat = value;
                OnPropertyChanged(nameof(CurrentChat));
            }
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    public async Task<IEnumerable<string>> GetConversations() {
        var response = await client.GetAsync($"/messages/all");
        return await response.Content.ReadFromJsonAsync<ObservableCollection<string>>() ?? new ObservableCollection<string>();
    }

    public async Task<IEnumerable<Message>> InitializeMessagesAsync(string otherUser) {
        var response = await client.GetAsync($"/messages/{otherUser}");
        return await response.Content.ReadFromJsonAsync<ObservableCollection<Message>>() ?? new ObservableCollection<Message>();
    }

    public async Task<bool> SendMessageAsync(Message message) {
        var response = await client.PostAsync($"/messages", message);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> CheckUser(string newConversationName) {
        var response = await client.GetAsync($"/check/{newConversationName}");
        return await response.Content.ReadFromJsonAsync<bool>();
    }
}