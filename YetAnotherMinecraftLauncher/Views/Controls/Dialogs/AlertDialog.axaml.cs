using Avalonia;
using Avalonia.Controls;
using System.Windows.Input;

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
    }
}
