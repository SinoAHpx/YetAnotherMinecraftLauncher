using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using System.Windows.Input;
using DialogHostAvalonia;
using ReactiveUI;

namespace YetAnotherMinecraftLauncher.Views.Controls.Dialogs
{
    public partial class AlertDialog : UserControl
    {
        public AlertDialog()
        {
            InitializeComponent();
            Grid.DataContext = this;
        }

        public static readonly StyledProperty<ICommand> DismissActionCommandProperty =
            AvaloniaProperty.Register<AlertDialog, ICommand>(nameof(DismissActionCommand));

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

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<AlertDialog, string>(nameof(Message));

        public async Task ShowDialogAsync(string message)
        {
            Message = message;
            DismissActionCommand = ReactiveCommand.Create(() => DialogHost.Close(null));

            await DialogHost.Show(this);
        }
    }
}
