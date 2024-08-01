using System;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace YetAnotherMinecraftLauncher.Models.Data;

internal static class DefaultAssets
{
    public static Bitmap AccountAvatar => new(AssetLoader.Open(
        new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultAccountAvatar.png")));

    public static Bitmap VersionAvatar => new(AssetLoader.Open(
        new Uri("avares://YetAnotherMinecraftLauncher/Assets/DefaultVersionAvatar.webp")));
}