using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using DialogHostAvalonia;
using ReactiveUI;

namespace YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

public partial class AlertDialog : UserControl
{
    public static readonly StyledProperty<ICommand> DismissActionCommandProperty =
        AvaloniaProperty.Register<AlertDialog, ICommand>(nameof(DismissActionCommand));

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<AlertDialog, string>(nameof(Message));

    public AlertDialog()
    {
        InitializeComponent();
        Grid.DataContext = this;
    }

    public ICommand DismissActionCommand
    {
        get => GetValue(DismissActionCommandProperty);
        set => SetValue(DismissActionCommandProperty, value);
    }


    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public async Task ShowDialogAsync(string message)
    {
        Message = message;
        DismissActionCommand = ReactiveCommand.Create(() => DialogHost.Close(null));

        await DialogHost.Show(this);
    }
}