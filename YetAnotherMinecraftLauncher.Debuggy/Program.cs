using Manganese.Array;
using Manganese.Process;
using ModuleLauncher.NET.Authentications;
using System;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using YetAnotherMinecraftLauncher.Utils;

namespace YetAnotherMinecraftLauncher.Debugy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            while (true)
            {
                var file = Console.ReadLine();
                var version = ConfigUtils.GetJavaVersion(file);
                Console.WriteLine(version);
            }
        }
    }
}
