﻿using System;
using Material.Colors;

namespace YetAnotherMinecraftLauncher.Utils;

internal static class ViewUtils
{
    public static PrimaryColor MapPrimaryColor(this string color)
    {
        return color.ToLower() switch
        {
            "red" => PrimaryColor.Red,
            "pink" => PrimaryColor.Pink,
            "purple" => PrimaryColor.Purple,
            "deeppurple" => PrimaryColor.DeepPurple,
            "indigo" => PrimaryColor.Indigo,
            "blue" => PrimaryColor.Blue,
            "lightblue" => PrimaryColor.LightBlue,
            "cyan" => PrimaryColor.Cyan,
            "teal" => PrimaryColor.Teal,
            "green" => PrimaryColor.Green,
            "lightgreen" => PrimaryColor.LightGreen,
            "lime" => PrimaryColor.Lime,
            "yellow" => PrimaryColor.Yellow,
            "amber" => PrimaryColor.Amber,
            "orange" => PrimaryColor.Orange,
            "brown" => PrimaryColor.Brown,
            _ => throw new ArgumentException("Invalid color", nameof(color))
        };
    }
}