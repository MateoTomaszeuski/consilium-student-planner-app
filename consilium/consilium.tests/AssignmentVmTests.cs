using Consilium.Shared.Models;
using Consilium.Shared.Services;
using Consilium.Shared.ViewModels;
using NSubstitute;
using System.Collections.ObjectModel;
namespace Consilium.Tests;


public class AssignmentsVmTest {
    private AssignmentsViewModel viewModel;
    private IAssignmentService assignmentService;

    public AssignmentsVmTest() {
        assignmentService = Substitute.For<IAssignmentService>();
        viewModel = new AssignmentsViewModel(assignmentService, Substitute.For<ILogInService>(), Substitute.For<IToDoService>());

    }

    [Before(Test)]
    public void Setup() {
        assignmentService.AddCourseAsync(Arg.Any<Course>())
            .Returns(Task.CompletedTask);

        assignmentService.GetAllCoursesAsync()
            .Returns((new List<Course> {
            new Course { CourseName = "Math" }
            }));

        viewModel = new AssignmentsViewModel(assignmentService, Substitute.For<ILogInService>(), Substitute.For<IToDoService>());
    }

    [Test]
    public async Task CanCreateViewModel() {
        await Assert.That(viewModel).IsNotNull();
    }

    [Test]
    public async Task CanCreateCourse() {
        viewModel.NewCourseName = "Math";
        viewModel.AddCourseCommand.Execute(null);
        await Assert.That(viewModel.Courses.Count).IsEqualTo(1);
        await Assert.That(viewModel.Courses[0].CourseName).IsEqualTo("Math");
    }

    [Test]
    public async Task WhenCourseIsSelected_AssignmentsAreFiltered() {
        assignmentService.AllAssignments = new List<Assignment> {
            new Assignment { CourseId = 1, Name = "Math Homework", Description = "do math homework stuff" },
            new Assignment { CourseId = 2, Name = "History Essay", Description = "write history essay" }
        };

        viewModel.SelectedCourse = new Course { Id = 1, CourseName = "Math" };
        await Assert.That(viewModel.Assignments.Count).IsEqualTo(1);
    }

    [Test]
    public async Task AssignmentsCanOnlyBeAddedWhenTitleIsNotEmpty() {
        viewModel.NewAssignmentTitle = "";
        await Assert.That(viewModel.AddAssignmentCommand.CanExecute(null)).IsFalse();

        viewModel.NewAssignmentTitle = "Essay";
        await Assert.That(viewModel.AddAssignmentCommand.CanExecute(null)).IsTrue();
    }


    [Test]
    public async Task WhenCourseIsSelected_AssignmentFormCanBeToggled() {
        viewModel.SelectedCourse = new Course { Id = 1, CourseName = "Math" };
        viewModel.SelectedCourse = new Course { Id = 1, CourseName = "Math2" };
        await Assert.That(viewModel.ToggleAssignmentFormCommand.CanExecute(null)).IsTrue();
    }

    [Test]
    public async Task WhenNewAssignmentIsCreated_FormInputsAreReset() {
        viewModel.NewAssignmentTitle = "Math Homework";
        viewModel.NewAssignmentDescription = "do math homework stuff";
        viewModel.NewAssignmentDueDate = DateTime.Now;

        await Assert.That(viewModel.NewAssignmentTitle).IsEqualTo("Math Homework");
        await Assert.That(viewModel.NewAssignmentDescription).IsEqualTo("do math homework stuff");
        await Assert.That(viewModel.NewAssignmentDueDate).IsNotNull();

        viewModel.AddAssignmentCommand.Execute(null);

        await Assert.That(viewModel.NewAssignmentTitle).IsEqualTo(string.Empty);
        await Assert.That(viewModel.NewAssignmentDescription).IsEqualTo(string.Empty);
    }

    [Test]
    public async Task WhenAssignmentIsMarkedComplete_CompletionDateIsNotNull() {
        viewModel.Courses.Add(new Course { Id = 1, CourseName = "Math" });

        viewModel.Assignments = new ObservableCollection<Assignment> {
            new Assignment { Name = "Math Homework", Description = "do math homework stuff", CourseId = 1 }
        };

        var assignment = viewModel.Assignments[0];
        await Assert.That(assignment.DateCompleted).IsNull();

        assignment.IsCompleted = true;

        await Assert.That(assignment.DateCompleted).IsNotNull();
    }

    [Test]
    public async Task AddCourse_ThenPreventDuplicateCourse() {
        viewModel.NewCourseName = "Math";
        viewModel.AddCourseCommand.Execute(null);

        await Assert.That(viewModel.Courses.Count).IsEqualTo(1);
        await Assert.That(viewModel.Courses[0].CourseName).IsEqualTo("Math");
        await Assert.That(viewModel.SelectedCourse).IsNotNull();
        await Assert.That(viewModel.SelectedCourse.CourseName).IsEqualTo("Math");
        await assignmentService.Received(1).AddCourseAsync(Arg.Is<Course>(c => c.CourseName == "Math"));

        viewModel.NewCourseName = "Math";
        viewModel.AddCourseCommand.Execute(null);

        await Assert.That(viewModel.Courses.Count).IsEqualTo(1);
        await assignmentService.Received(1).AddCourseAsync(Arg.Is<Course>(c => c.CourseName == "Math"));
    }
}