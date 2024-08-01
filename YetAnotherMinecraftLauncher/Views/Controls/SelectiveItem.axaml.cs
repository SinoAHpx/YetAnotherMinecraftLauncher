using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace YetAnotherMinecraftLauncher.Views.Controls;

public partial class SelectiveItem : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<SelectiveItem, string>(nameof(Title));

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<SelectiveItem, string>(nameof(Subtitle));

    public static readonly StyledProperty<IImageBrushSource> AvatarProperty =
        AvaloniaProperty.Register<SelectiveItem, IImageBrushSource>(nameof(Avatar));

    public static readonly StyledProperty<ICommand> SelectActionProperty =
        AvaloniaProperty.Register<SelectiveItem, ICommand>(nameof(SelectAction));

    public static readonly StyledProperty<ICommand> RemoveActionProperty =
        AvaloniaProperty.Register<SelectiveItem, ICommand>(nameof(RemoveAction));

    public SelectiveItem()
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

    public ICommand SelectAction
    {
        get => GetValue(SelectActionProperty);
        set => SetValue(SelectActionProperty, value);
    }

    public ICommand RemoveAction
    {
        get => GetValue(RemoveActionProperty);
        set => SetValue(RemoveActionProperty, value);
    }
}