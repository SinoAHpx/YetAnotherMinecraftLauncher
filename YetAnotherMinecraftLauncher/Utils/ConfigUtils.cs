using System;
using System.IO;
using System.Threading.Tasks;
using Manganese.IO;
using Manganese.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
    }

    // here we have 2 different configs,
    // one is MinecraftSettings and other is LauncherSettings
    public static async Task WriteConfigAsync<T>(this T type)
    {
        var jsonText = type.ToJsonString();

        await ConfigFile.WriteAllTextAsync(jsonText);
    }

    /// <summary>
    /// this did nothing but just to read the raw json string
    /// </summary>
    /// <returns></returns>
    public static string? ReadConfig()
    {
        var configText = ConfigFile.ReadAllText();

        return configText == string.Empty ? null : configText;
    }
}