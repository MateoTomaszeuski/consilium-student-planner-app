using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using Consilium.Shared.Services;
using System.Collections.ObjectModel;

namespace Consilium.Shared.ViewModels;
public partial class ChatViewModel(IMessageService messageService, ILogInService logInService) : ObservableObject {

    [ObservableProperty]
    private ObservableCollection<string> conversations = new();
    [ObservableProperty]
    private bool showConversations = true;
    [ObservableProperty]
    private string selectedConversation = string.Empty;
    [ObservableProperty]
    private bool showChat = false;
    [ObservableProperty]
    private bool isNotCreatingNewConversation = true;

    [ObservableProperty]
    private bool isCreatingNewConversation = false;
    [ObservableProperty]
    private string newConversationName = string.Empty;
    [ObservableProperty]
    private string displayMessage = string.Empty;
    [ObservableProperty]
    private bool online = false;
    [ObservableProperty]
    private string onlineMessage = string.Empty;
    public async Task InitConversations() {
        Online = await logInService.CheckAuthStatus();
        if (!Online) {
            OnlineMessage = "You are in Guest mode, make sure to login or connect to the internet";
            return;
        }
        Conversations = new(await messageService.GetConversations());
    }

    [RelayCommand]
    private void SelectConversation(string conversation) {
        if (Conversations.Contains(conversation)) {
            messageService.CurrentChat = conversation;
            ShowConversations = false;
            ShowChat = true;
        }
    }

    [RelayCommand]
    private void Back() {
        ShowConversations = true;
        ShowChat = false;
        WeakReferenceMessenger.Default.Send(new ClearMessagesRequest());
        NewConversationName = string.Empty;
        messageService.CurrentChat = string.Empty;
        IsCreatingNewConversation = false;
        IsNotCreatingNewConversation = true;
    }

    [RelayCommand]
    private void ActivateNewConversation() {
        IsCreatingNewConversation = true;
        IsNotCreatingNewConversation = false;
    }

    [RelayCommand]
    private async Task CreateConversation() {
        var uservalid = await messageService.CheckUser(NewConversationName);
        if (string.IsNullOrWhiteSpace(NewConversationName)
            || Conversations.Contains(NewConversationName)
            || !uservalid) {
            DisplayMessage = "Invalid conversation name or user does not exist.";
            return;
        }
        Conversations.Add(NewConversationName);
        SelectConversation(NewConversationName);

        IsCreatingNewConversation = false;
        IsNotCreatingNewConversation = true;

    }
}