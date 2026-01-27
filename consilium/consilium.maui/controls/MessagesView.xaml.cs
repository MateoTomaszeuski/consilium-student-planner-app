using Consilium.Shared.ViewModels.Controls;
using System.Collections.Specialized;

namespace Consilium.Maui.Controls;

public partial class MessagesView : ContentView {
    private MessagesViewModel vm;

    public MessagesView() {
        InitializeComponent();
        vm = ((App)Application.Current).Services.GetService<MessagesViewModel>();
        BindingContext = vm;

        MessagesCollectionView.Loaded += MessagesCollectionView_Loaded;
        vm.MessagesUpdated += OnMessagesUpdated;
    }

    private void MessagesCollectionView_Loaded(object? sender, EventArgs e) {
        ScrollToLastMessage();

        if (BindingContext is MessagesViewModel vm
         && vm.AllMessages is INotifyCollectionChanged notify) {
            notify.CollectionChanged += OnMessagesCollectionChanged;
        }
    }
    private void OnMessagesCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
        if (e.Action == NotifyCollectionChangedAction.Add)
            ScrollToLastMessage();
    }

    private void ScrollToLastMessage() {
        if (MessagesCollectionView.ItemsSource is System.Collections.IList list && list.Count > 0) {
            var lastItem = list[list.Count - 1];
            MessagesCollectionView.ScrollTo(lastItem, position: ScrollToPosition.End, animate: true);
        }
    }
    private void OnMessagesUpdated() {
        ScrollToLastMessage();
    }
}