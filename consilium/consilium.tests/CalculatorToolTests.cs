using Consilium.Shared.ViewModels.Controls;

namespace Consilium.Tests;

public class CalculatorToolTests {
    private CalculatorViewModel viewModel;
    public CalculatorToolTests() {
        viewModel = new CalculatorViewModel();
    }

    [Before(Test)]
    public void Setup() {
        viewModel = new CalculatorViewModel();
    }

    [Test]
    public async Task Digit_ShouldSetDisplayText() {
        string expected = "7";
        viewModel.Digit("7");
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);

        expected = "73";
        viewModel.Digit("3");
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Operator_FirstOperatorWithNoPreviousInput_ShouldDisplayOperator() {
        string expected = "+";
        viewModel.Operator("+");
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Operator_AfterNumber_ShouldAppendOperator() {
        string expected = "5*";
        viewModel.Digit("5");
        viewModel.Operator("*");
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Operator_ReplacePreviousOperator_WhenCalledTwice_ShouldSwap() {
        string expected = "8/";
        viewModel.Digit("8");
        viewModel.Operator("-");
        viewModel.Operator("/");
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Equals_Addition_Works() {
        string expected = "5";
        viewModel.Digit("2");
        viewModel.Operator("+");
        viewModel.Digit("3");
        viewModel.Equals();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Equals_Subtraction_Works() {
        string expected = "5";
        viewModel.Digit("9");
        viewModel.Operator("-");
        viewModel.Digit("4");
        viewModel.Equals();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Equals_Multiplication_Works() {
        string expected = "42";
        viewModel.Digit("6");
        viewModel.Operator("*");
        viewModel.Digit("7");
        viewModel.Equals();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Equals_Division_Works() {
        string expected = "4";
        viewModel.Digit("8");
        viewModel.Operator("/");
        viewModel.Digit("2");
        viewModel.Equals();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Equals_DivisionByZero_ReturnsZero() {
        string expected = "0";
        viewModel.Digit("8");
        viewModel.Operator("/");
        viewModel.Digit("0");
        viewModel.Equals();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Clear_ShouldResetCalculator() {
        string expected = "0";
        viewModel.Digit("1");
        viewModel.Operator("+");
        viewModel.Digit("2");
        viewModel.Clear();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Backspace_WhenInputNotEmpty_RemovesLastCharacter() {
        viewModel.Digit("1");
        viewModel.Digit("2");
        viewModel.Digit("3");

        string expected = "12";
        viewModel.Backspace();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);

        expected = "1";
        viewModel.Backspace();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);

        expected = "0";
        viewModel.Backspace();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }

    [Test]
    public async Task Backspace_WhenInputEmpty_DoesNothing() {
        string expected = "0";
        viewModel.Clear();
        viewModel.Backspace();
        await Assert.That(viewModel.DisplayText).IsEqualTo(expected);
    }
}