using Manganese.Array;
using Manganese.Process;
using ModuleLauncher.NET.Authentications;
using System;
using System.Buffers.Text;
using System.Security.Cryptography;
using System.Text;
using ModuleLauncher.NET.Models.Launcher;
using ModuleLauncher.NET.Models.Resources;
using ModuleLauncher.NET.Resources;
using ModuleLauncher.NET.Utilities;
using YetAnotherMinecraftLauncher.Utils;
using Manganese.Text;
using Microsoft.IdentityModel.Tokens;
using Console = System.Console;

namespace YetAnotherMinecraftLauncher.Debugy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            await AccountUtils.EncryptAsync(File.ReadAllText(
                @"C:\Codebase\Dotnet\YetAnotherMinecraftLauncher\YetAnotherMinecraftLauncher.Desktop\bin\Debug\net8.0\yaml.json"));

            Console.WriteLine(await AccountUtils.DecryptAsync());
        }

        private static Aes _aes = Aes.Create();
        private static string _rootDir = @"C:\Users\ahpx\Documents\testground";

        private static async Task Write(string plainText)
        {
            _aes.GenerateKey();
            await File.WriteAllBytesAsync(_rootDir.CombinePath("key"), _aes.Key);
            _aes.GenerateIV();
            await File.WriteAllBytesAsync(_rootDir.CombinePath("iv"), _aes.IV);
            var encryptCfb = _aes.EncryptCfb(Encoding.Default.GetBytes(plainText), _aes.IV);
            await File.WriteAllBytesAsync(_rootDir.CombinePath("pwd"), encryptCfb);
        }

        private static void Read()
        {
            var key = File.ReadAllText(_rootDir.CombinePath("key"));
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
