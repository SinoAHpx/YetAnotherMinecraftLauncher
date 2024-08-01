using System;
using System.IO;
using Manganese.Text;

namespace YetAnotherMinecraftLauncher.Models.Data;

internal static class DataPaths
{
    public static (string value, string iv, string key) GetEncryptionPaths()
    {
        var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        return (root.CombinePath("eced_value"), root.CombinePath("eced_iv"), root.CombinePath("eced_key"));
    }

    public static FileInfo GetConfigFile()
    {
        return new FileInfo(Directory.GetCurrentDirectory().CombinePath("yaml.json"));
    }
}