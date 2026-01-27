using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Consilium.Shared.Models;
using Consilium.Shared.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Consilium.Shared.ViewModels.Controls;

public partial class MessagesViewModel : ObservableObject {
    public MessagesViewModel(IMessageService messageService, IPersistenceService persistenceService, ILogInService logInService) {
        this.messageService = messageService;
        this.logInService = logInService;
        messageService.PropertyChanged += MessageService_PropertyChanged;
        MyUserName = persistenceService.GetUserName();

        WeakReferenceMessenger.Default.Register<ClearMessagesRequest>(this, (r, m) =>
        {
            AllMessages.Clear();
        });

    }

    [ObservableProperty]
    private string myUserName = string.Empty;

    [ObservableProperty]
    private ObservableCollection<Message> allMessages = new();

    [ObservableProperty]
    private string conversationWith = string.Empty;
    [ObservableProperty]
    private string messageContent = string.Empty;

    private readonly IMessageService messageService;
    private readonly ILogInService logInService;

    public event Action? MessagesUpdated;

    private async void MessageService_PropertyChanged(object? sender, PropertyChangedEventArgs e) {
        if (e.PropertyName == nameof(messageService.CurrentChat)) {
            if (messageService.CurrentChat != null) {
                ConversationWith = messageService.CurrentChat;
                await InitializeMessagesAsync();
            }
        }
    }

    [RelayCommand]
    public async Task SendMessage() {
        if (string.IsNullOrEmpty(MessageContent)) {
            return;
        }

        var message = new Message {
            Sender = MyUserName,
            Receiver = ConversationWith,
            Content = MessageContent,
            TimeSent = DateTime.Now,
            IsMyMessage = true
        };

        var sent = await messageService.SendMessageAsync(message);
        if (sent) {
            AllMessages.Add(message);
            MessageContent = string.Empty;
        }

        MessagesUpdated?.Invoke();
    }

    [RelayCommand]
    public async Task InitializeMessagesAsync() {
        if (!string.IsNullOrEmpty(ConversationWith)) {
            var messages = await messageService.InitializeMessagesAsync(ConversationWith);
            AllMessages.Clear();
            foreach (var message in messages) {
                message.IsMyMessage = message.Sender == MyUserName;
                AllMessages.Add(message);
            }

            MessagesUpdated?.Invoke();
        }
    }
}