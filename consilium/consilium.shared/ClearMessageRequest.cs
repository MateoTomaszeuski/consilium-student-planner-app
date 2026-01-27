namespace Consilium.Shared;
public class ClearMessagesRequest : CommunityToolkit.Mvvm.Messaging.Messages.ValueChangedMessage<bool> {
    public ClearMessagesRequest() : base(true) { }
}