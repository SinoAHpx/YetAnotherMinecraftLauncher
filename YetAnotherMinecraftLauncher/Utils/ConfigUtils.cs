using System;
using System.IO;
using System.Threading.Tasks;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;

namespace YetAnotherMinecraftLauncher.Utils;

public static class ConfigUtils
{
    public static FileInfo ConfigFile { get; set; } =
        new (Path.Combine(Directory.GetCurrentDirectory(), "yaml.json"));

    static ConfigUtils()
    {
        if (!ConfigFile.Exists)
        {
            using var _ = ConfigFile.Create();
        }
        ConfigText = ConfigFile.ReadAllText();

    }

    // here we have 2 different configs,
    // one is MinecraftSettings and other is LauncherSettings
    public static async Task WriteConfigAsync<T>(this T type)
    {
        var jsonText = type.ToJsonString();

        await ConfigFile.WriteAllTextAsync(jsonText);
    }

    public static string ConfigText;

    public static string? ReadConfig(string key)
    {
        
        ConfigFile
            .WhenAnyValue(x => x.LastWriteTime)
            .Subscribe(s => ConfigText = ConfigFile.ReadAllText());

        var configStr = ConfigText.Fetch(key);
        return configStr;
    }



    public static string MinecraftDirectory { get; set; }

    private static MinecraftResolver? _minecraftResolver;

    public static MinecraftResolver GetMinecraftResolver()
    {
        return _minecraftResolver ??= new MinecraftResolver(MinecraftDirectory);
    }
}