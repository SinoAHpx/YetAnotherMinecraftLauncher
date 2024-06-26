using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using System.Windows.Input;

namespace YetAnotherMinecraftLauncher.Views.Controls
{
    public partial class SelectiveItem : UserControl
    {
        public SelectiveItem()
        {
            InitializeComponent();

            Card.DataContext = this;
        }

        public static readonly StyledProperty<string> TitleProperty =
            AvaloniaProperty.Register<InteractiveItem, string>(nameof(Title));


        public string Title
        {
            get => GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public static readonly StyledProperty<string> SubtitleProperty =
            AvaloniaProperty.Register<InteractiveItem, string>(nameof(Subtitle));


        public string Subtitle
        {
            get => GetValue(SubtitleProperty);
            set => SetValue(SubtitleProperty, value);
        }

        public static readonly StyledProperty<IImageBrushSource> AvatarProperty =
            AvaloniaProperty.Register<InteractiveItem, IImageBrushSource>(nameof(Avatar));

        public IImageBrushSource Avatar
        {
            get => GetValue(AvatarProperty);
            set => SetValue(AvatarProperty, value);
        }

        public static readonly StyledProperty<ICommand> SelectActionProperty =
            AvaloniaProperty.Register<InteractiveItem, ICommand>(nameof(SelectAction));

        public ICommand SelectAction
        {
            get => GetValue(SelectActionProperty);
            set => SetValue(SelectActionProperty, value);
        }

        public static readonly StyledProperty<ICommand> RemoveActionProperty =
            AvaloniaProperty.Register<InteractiveItem, ICommand>(nameof(RemoveAction));

        public ICommand RemoveAction
        {
            get => GetValue(RemoveActionProperty);
            set => SetValue(RemoveActionProperty, value);
        }
    }
}
