using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace YetAnotherMinecraftLauncher.Views.Controls.Dialogs
{
    public partial class ConfirmDialog : UserControl
    {
        public ConfirmDialog()
        {
            InitializeComponent();
            Grid.DataContext = this;
        }


        public static readonly StyledProperty<ICommand> CancelActionCommandProperty =
            AvaloniaProperty.Register<ConfirmDialog, ICommand>(nameof(CancelActionCommand));

        public ICommand CancelActionCommand
        {
            get => GetValue(CancelActionCommandProperty);
            set => SetValue(CancelActionCommandProperty, value);
        }

        public static readonly StyledProperty<ICommand> ConfirmActionCommandProperty =
            AvaloniaProperty.Register<ConfirmDialog, ICommand>(nameof(ConfirmActionCommand));

        public ICommand ConfirmActionCommand
        {
            get => GetValue(ConfirmActionCommandProperty);
            set => SetValue(ConfirmActionCommandProperty, value);
        }


        public string Message
        {
            get => GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        public static readonly StyledProperty<string> MessageProperty =
            AvaloniaProperty.Register<ConfirmDialog, string>(nameof(Message));
    }
}
