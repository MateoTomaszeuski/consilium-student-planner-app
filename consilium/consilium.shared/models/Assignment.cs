using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Consilium.Shared.Models;

public partial class Assignment : ObservableObject {
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int CourseId { get; set; }
    public DateTime? DueDate { get; set; }
    public DateTime? DateStarted { get; set; }
    public DateTime? DateCompleted { get; set; }
    public bool HasDescription => !String.IsNullOrEmpty(Description);

    [ObservableProperty]
    private bool isCompleted;

    [ObservableProperty]
    private bool descriptionIsExpanded;



    partial void OnIsCompletedChanged(bool value) {
        if (value) {
            DateCompleted = DateTime.Now;
        } else {
            DateCompleted = null;
        }
    }

    [RelayCommand]
    public void ToggleDescription() {
        DescriptionIsExpanded = !DescriptionIsExpanded;
    }


}