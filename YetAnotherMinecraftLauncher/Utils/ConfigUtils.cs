using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using YetAnotherMinecraftLauncher.ViewModels;

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

        if (ConfigText.IsNullOrEmpty())
        {
            return null;
        }

        var configStr = ConfigText.Fetch(key);
        return configStr;
    }

    public static bool CheckConfig()
    {
        var type = typeof(SettingsViewModel);
        var properties = type.GetProperties();
        foreach (var propertyInfo in properties)
        {
            foreach (var customAttribute in propertyInfo.GetCustomAttributes())
            {
                if (customAttribute is ValidationAttribute validation)
                {
                    if (!validation.IsValid(ReadConfig(propertyInfo.Name)))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private static MinecraftResolver? _resolverCache;

    public static MinecraftResolver? GetMinecraftResolver()
    {
        if (_resolverCache != null)
            return _resolverCache;

        var minecraftDirectoryType = ReadConfig("MinecraftDirectoryType");
        if (minecraftDirectoryType.IsNullOrEmpty())
        {
            return null;
        }
        var customMinecraftDirectory = ReadConfig("CustomMinecraftDirectory");
        if (customMinecraftDirectory.IsNullOrEmpty())
        {
            return null;
        }

        if (!Directory.Exists(customMinecraftDirectory))
        {
            return null;
        }

        _resolverCache = minecraftDirectoryType.ToInt32() == 0
            ? new MinecraftResolver(".minecraft")
            : new MinecraftResolver(customMinecraftDirectory);

        Directory.CreateDirectory(_resolverCache.RootPath.CombinePath("versions"));

        if (!Directory.Exists(_resolverCache.RootPath))
        {
            Directory.CreateDirectory(_resolverCache.RootPath);
        }

        return _resolverCache;
    }
}