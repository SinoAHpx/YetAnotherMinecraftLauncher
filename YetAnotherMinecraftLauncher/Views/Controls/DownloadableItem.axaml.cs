using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;

namespace YetAnotherMinecraftLauncher.Views.Controls;

public partial class DownloadableItem : UserControl
{
    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<DownloadableItem, string>(nameof(Title));

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<DownloadableItem, string>(nameof(Subtitle));

    public static readonly StyledProperty<IImageBrushSource> AvatarProperty =
        AvaloniaProperty.Register<DownloadableItem, IImageBrushSource>(nameof(Avatar));

    public static readonly StyledProperty<ICommand> DownloadActionProperty =
        AvaloniaProperty.Register<DownloadableItem, ICommand>(nameof(DownloadAction));

    public DownloadableItem()
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

    public ICommand DownloadAction
    {
        get => GetValue(DownloadActionProperty);
        set => SetValue(DownloadActionProperty, value);
    }
}