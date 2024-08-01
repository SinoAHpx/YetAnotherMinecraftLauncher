using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Manganese.IO;
using Manganese.Text;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Resources;
using Newtonsoft.Json.Linq;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Config;
using YetAnotherMinecraftLauncher.Models.Data;
using YetAnotherMinecraftLauncher.ViewModels;

namespace YetAnotherMinecraftLauncher.Utils;

public static class ConfigUtils
{
    public static string ConfigText;

    static ConfigUtils()
    {
        if (!ConfigFile.Exists)
        {
            using var _ = ConfigFile.Create();
        }

        ConfigText = ConfigFile.ReadAllText();
    }

    public static FileInfo ConfigFile { get; set; } = DataPaths.GetConfigFile();

    // here we have 2 different configs,
    // one is MinecraftSettings and other is LauncherSettings
    public static async Task WriteConfigAsync<T>(this T type, ConfigNodes nodeName)
    {
        try
        {
            if (ConfigText.IsNullOrEmpty())
            {
                var container = new JObject { { nodeName.ToString(), type.ToJsonString().ToJObject() } };
                await ConfigFile.WriteAllTextAsync(container.ToString());
            }

            var jo = JObject.Parse(ConfigText);
            jo[nodeName.ToString()] = type.ToJsonString().ToJObject();

            var jsonString = jo.ToString();
            await ConfigFile.WriteAllTextAsync(jsonString);
        }
        catch
        {
            ConfigFile.Delete();
            await WriteConfigAsync(type, nodeName);
        }
    }

    public static string? ReadConfig(string key, ConfigNodes node = ConfigNodes.Settings)
    {
        ConfigFile
            .WhenAnyValue(x => x.LastWriteTime)
            .Subscribe(_ => ConfigText = ConfigFile.ReadAllText());
        if (key.IsNullOrEmpty()) return ConfigText;

        var configStr = ConfigText.Fetch($"{node.ToString()}.{key}");
        return configStr;
    }

    public static bool CheckConfig()
    {
        var type = typeof(SettingsViewModel);
        var properties = type.GetProperties();
        foreach (var propertyInfo in properties)
        foreach (var customAttribute in propertyInfo.GetCustomAttributes())
            if (customAttribute is ValidationAttribute validation)
                if (!validation.IsValid(ReadConfig(propertyInfo.Name)))
                    return false;

        return true;
    }


    public static MinecraftResolver? GetMinecraftResolver()
    {
        var minecraftDirectoryType = ReadConfig("MinecraftDirectoryType");
        if (minecraftDirectoryType.IsNullOrEmpty()) return null;
        var customMinecraftDirectory = ReadConfig("CustomMinecraftDirectory");
        if (customMinecraftDirectory.IsNullOrEmpty()) return null;

        if (!Directory.Exists(customMinecraftDirectory)) return null;

        var resolver = minecraftDirectoryType.ToInt32() == 0
            ? new MinecraftResolver(".minecraft")
            : new MinecraftResolver(customMinecraftDirectory);

        Directory.CreateDirectory(resolver.RootPath.CombinePath("versions"));

        if (!Directory.Exists(resolver.RootPath)) Directory.CreateDirectory(resolver.RootPath);

        return resolver;
    }

    /// <summary>
    ///     If error occurred, an empty list will be returned.
    /// </summary>
    /// <returns></returns>
    public static List<MinecraftJava> GetJavas()
    {
        var rawTokens = ReadConfig("JavaExecutables");
        if (rawTokens.IsNullOrEmpty()) return [];

        try
        {
            var rawJavas = JArray.Parse(rawTokens);

            return rawJavas.Select(r => new MinecraftJava
            {
                Executable = new FileInfo(r.Fetch("Executable")!),
                Version = r.Fetch("Version")!.ToInt32()
            }).ToList();
        }
        catch
        {
            return [];
        }
    }

    public static int? GetJavaVersion(string executableFilePath)
    {
        if (executableFilePath.IsNullOrEmpty() || !File.Exists(executableFilePath)) return null;

        // /bin/javaw.exe
        var info = new FileInfo(executableFilePath);
        var javaRoot = info.Directory?.Parent;
        if (javaRoot == null) return null;

        try
        {
            var rootName = javaRoot.Name;
            var version = rootName.Split('-')[1];

            if (version.Contains('.')) return version.Split('.')[0].ToInt32();

            return version.ToInt32();
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    ///     This is launcher config without authentication info
    /// </summary>
    /// <returns></returns>
    public static LauncherConfig? GetLauncherConfig()
    {
        if (ConfigText.IsNullOrEmpty()) return null;
        if (!CheckConfig()) return null;

        try
        {
            var config = new LauncherConfig
            {
                WindowWidth = ReadConfig("WindowWidth").ToInt32(),
                WindowHeight = ReadConfig("WindowHeight").ToInt32(),
                Fullscreen = ReadConfig("IsFullscreen").ToBool(),
                DirectlyJoinServer = ReadConfig("DirectlyJoinServer").IsNullOrEmpty()
                    ? null
                    : ReadConfig("DirectlyJoinServer"),
                Javas = GetJavas(),
                MaxMemorySize = ReadConfig("AllocatedMemorySize").ToInt32(),
                LauncherName = "YAML"
            };


            return config;
        }
        catch
        {
            return null;
        }
    }
}