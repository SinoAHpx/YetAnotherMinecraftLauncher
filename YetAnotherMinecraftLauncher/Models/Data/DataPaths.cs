using Manganese.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherMinecraftLauncher.Models.Data
{
    internal static class DataPaths
    {
        public static (string value, string iv, string key) GetEncryptionPaths()
        {
            var root = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

            return (root.CombinePath("eced_value"), root.CombinePath("eced_iv"), root.CombinePath("eced_key"));
        }

        public static FileInfo GetConfigFile() 
            => new(Directory.GetCurrentDirectory().CombinePath("yaml.json"));
    }
}
