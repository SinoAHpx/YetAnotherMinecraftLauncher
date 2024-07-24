using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DialogHostAvalonia;
using YetAnotherMinecraftLauncher.Models.Attributes;
using YetAnotherMinecraftLauncher.Views.Controls.Dialogs;

namespace YetAnotherMinecraftLauncher;

public partial class InputDialog : UserControl
{
    public InputDialog()
    {
        InitializeComponent();

        Grid.DataContext = this;
    }

    public static readonly StyledProperty<ICommand> ConfirmActionCommandProperty =
        AvaloniaProperty.Register<InputDialog, ICommand>(nameof(ConfirmActionCommand));

    public ICommand ConfirmActionCommand
    {
        get => GetValue(ConfirmActionCommandProperty);
        set => SetValue(ConfirmActionCommandProperty, value);
    }

    public static readonly StyledProperty<ICommand> CancelActionCommandProperty =
        AvaloniaProperty.Register<InputDialog, ICommand>(nameof(CancelActionCommand));

    public ICommand CancelActionCommand
    {
        get => GetValue(CancelActionCommandProperty);
        set => SetValue(CancelActionCommandProperty, value);
    }

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<InputDialog, string>(nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public static readonly StyledProperty<string> MessageProperty =
        AvaloniaProperty.Register<InputDialog, string>(nameof(Message));

    public string Message
    {
        get => GetValue(MessageProperty);
        set => SetValue(MessageProperty, value);
    }

    public static readonly StyledProperty<string> InputProperty =
        AvaloniaProperty.Register<InputDialog, string>(nameof(Input));

    [Number]
    public string Input
    {
        get => GetValue(InputProperty);
        set => SetValue(InputProperty, value);
    }

    public async Task ShowDialogAsync()
    {
        await DialogHost.Show(this);
    }
}