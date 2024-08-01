using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace YetAnotherMinecraftLauncher.Views.Controls;

public partial class InteractiveItem : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<InteractiveItem, string>(nameof(Title));

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<InteractiveItem, string>(nameof(Subtitle));

    public static readonly StyledProperty<IImageBrushSource> AvatarProperty =
        AvaloniaProperty.Register<InteractiveItem, IImageBrushSource>(nameof(Avatar));

    public static readonly StyledProperty<ICommand> ActionProperty =
        AvaloniaProperty.Register<InteractiveItem, ICommand>(nameof(Action));

    public InteractiveItem()
    {
        InitializeComponent();
        Card.DataContext = this;
    }


    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }


    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public IImageBrushSource Avatar
    {
        get => GetValue(AvatarProperty);
        set => SetValue(AvatarProperty, value);
    }

    public ICommand Action
    {
        get => GetValue(ActionProperty);
        set => SetValue(ActionProperty, value);
    }
}