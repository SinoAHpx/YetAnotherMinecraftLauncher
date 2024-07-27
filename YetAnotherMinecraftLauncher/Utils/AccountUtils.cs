using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Manganese.IO;
using Manganese.Text;
using Microsoft.IdentityModel.Tokens;
using ModuleLauncher.NET.Models.Authentication;
using YetAnotherMinecraftLauncher.Models.Authentication;
using YetAnotherMinecraftLauncher.Views.Controls;

namespace YetAnotherMinecraftLauncher.Utils
{
    public static class AccountUtils
    {
        private static readonly Aes Aes = Aes.Create();

        private static readonly Dictionary<int, char> Map = new()
        {
            {1, 'a'}, {10, 'b'}, {11, 'c'}, {100, 'd'}, {101, 'e'}, {110, 'f'}, {111, 'g'}, {1000, 'h'}, {1001, 'i'}, {1010, 'j'},
            {1011, 'k'}, {1100, 'l'}, {1101, 'm'}, {1110, 'n'}, {1111, 'o'}, {10000, 'p'}, {10001, 'q'}, {10010, 'r'}, {10011, 's'}, {10100, 't'},
            {10101, 'u'}, {10110, 'v'}, {10111, 'w'}, {11000, 'x'}, {11001, 'y'}, {11010, 'z'}
        };

        public static async Task EncryptAsync(string plainText)
        {
            Aes.GenerateIV();

            var plainBytes = Encoding.Default.GetBytes(plainText);
            var encrypted = Aes.EncryptCfb(plainBytes, Aes.IV);

            var p = GetPaths();

            File.Delete(p.p1);
            await File.WriteAllBytesAsync(p.p1, encrypted);
            File.SetAttributes(p.p1, FileAttributes.Hidden);
            File.Delete(p.p2);
            await File.WriteAllBytesAsync(p.p2, Aes.IV);
            File.SetAttributes(p.p2, FileAttributes.Hidden);
        }

        public static async Task<string> DecryptAsync()
        {
            var p = GetPaths();

            var iv = await File.ReadAllBytesAsync(p.p2);
            Aes.IV = iv;

            var value = await File.ReadAllBytesAsync(p.p1);
            var de = Aes.DecryptCfb(value, iv);

            return Encoding.Default.GetString(de);
        }

        private static (string p1, string p2) GetPaths()
        {
            var result = GetResult();

            var path1 = result.CombinePath(new string([
                Map[101], Map[11], Map[101], Map[100]
            ]));
            var path2 = result.CombinePath(new string([
                Map[1001], Map[10110], '4'
            ]));

            return (path1, path2);
        }

        private static string GetResult()
        {
            var type = typeof(Environment);
            var result = string.Empty;
            foreach (var member in type.GetMembers())
            {
                var n = typeof(MemberInfo)
                    .GetProperties()
                    .Single(p => p.Name.ToLower() == new string([Map[1110], Map[1], Map[1101], Map[101]]));

                var v = n.GetValue(member) as string;
                var cp = new string(new[]
                {
                    Map[111], Map[101], Map[10100], Map[110], Map[1111], Map[1100], Map[100], Map[101],
                    Map[10010], Map[10000], Map[1], Map[10100], Map[1000]
                });
                if (v?.ToLower() == cp && member is MethodInfo method && method.GetParameters().Length == Map.Keys.First())
                {
                    result =
                        method.Invoke(null,
                        [
                            Environment.SpecialFolder.ApplicationData
                        ]) as string;
                }
            }

            return result!;
        }
    }
}
