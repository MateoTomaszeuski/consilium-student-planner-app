using CommunityToolkit.Mvvm.ComponentModel;
using Consilium.Shared.Services;


namespace Consilium.Shared.ViewModels.Controls;

public partial class NotesViewModel : ObservableObject, IDisposable {
    private readonly IPersistenceService service;
    [ObservableProperty]
    private string title = string.Empty;

    [ObservableProperty]
    private string content = string.Empty;

    public NotesViewModel(IPersistenceService service) {
        Content = service.GetNotes();
        this.service = service;
    }

    public void Dispose() {
        service.SaveNotes(Content);
    }
}