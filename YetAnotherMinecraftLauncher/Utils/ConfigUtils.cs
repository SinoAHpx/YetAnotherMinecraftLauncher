using System;
using System.IO;
using System.Threading.Tasks;
using Manganese.IO;
using Manganese.Text;
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

    public static async Task WriteConfigAsync<T>(this T type)
    {
        // var configText = await ConfigFile.ReadAllTextAsync();
        // var configJObject = JObject.Parse(configText);

        var jsonString = type.ToJsonString();
        await ConfigFile.WriteAllTextAsync(jsonString);
    }

    public static string? ReadConfigAsync()
    {
        var configText = ConfigFile.ReadAllText();
        if (configText == string.Empty)
        {
            return null;
        }

        return configText;
    }
}