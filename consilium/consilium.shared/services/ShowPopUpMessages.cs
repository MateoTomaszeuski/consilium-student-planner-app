using CommunityToolkit.Mvvm.Messaging.Messages;

namespace Consilium.Shared.Services;
public class ShowPopupMessage : SimpleMessage {
    public ShowPopupMessage() : base("ShowPopup") { }
}

public class SimpleMessage : ValueChangedMessage<string> {
    public SimpleMessage(string message) : base(message) {
    }
}