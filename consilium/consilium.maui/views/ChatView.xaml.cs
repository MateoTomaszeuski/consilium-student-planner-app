using Consilium.Shared.ViewModels;

namespace Consilium.Maui.Views;

public partial class ChatView : ContentPage {
    public ChatView(ChatViewModel vm) {
        InitializeComponent();
        BindingContext = vm;
        Appearing += async (s, e) =>
        {
            await vm.InitConversations();
        };
    }
}