using Manganese.Array;
using Manganese.Process;
using ModuleLauncher.NET.Authentications;
using System;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using YetAnotherMinecraftLauncher.Utils;

namespace YetAnotherMinecraftLauncher.Debugy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var process = await new MinecraftResolver(@"C:\Users\ahpx\AppData\Roaming\.minecraft")
                .GetMinecraft("1.16.2")
                .WithAuthentication("AHpx")
                .WithJavas(Javas)
                .WithWindowHeight(1080)
                .WithWindowWidth(1920)
                .WithDirectServer(null)
                .LaunchAsync();

            while (await process.ReadOutputLineAsync() is {} output)
            {
                Console.WriteLine(output);
            }
        }

        private static List<MinecraftJava> Javas =
        [
            new()
            {
                Executable = new FileInfo(@"C:\Program Files\Eclipse Adoptium\jre-21.0.3.9-hotspot\bin\javaw.exe"),
                Version = 21
            },
            new()
            {
                Executable = new FileInfo(@"C:\Program Files\Eclipse Adoptium\jre-17.0.11.9-hotspot\bin\javaw.exe"),
                Version = 17
            },
            new()
            {
                Executable = new FileInfo(@"C:\Program Files\Eclipse Adoptium\jre-11.0.23.9-hotspot\bin\javaw.exe"),
                Version = 11
            },
            new()
            {
                Executable = new FileInfo(@"C:\Program Files\Eclipse Adoptium\jre-8.0.412.8-hotspot\bin\javaw.exe"),
                Version = 8
            }
        ];
    }
}
