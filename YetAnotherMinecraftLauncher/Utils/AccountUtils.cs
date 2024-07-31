using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Manganese.Text;
using ModuleLauncher.NET.Models.Authentication;

namespace YetAnotherMinecraftLauncher.Utils
{
    public static class AccountUtils
    {
        private static readonly Dictionary<int, char> Map = new()
        {
            {1, 'a'}, {10, 'b'}, {11, 'c'}, {100, 'd'}, {101, 'e'}, {110, 'f'}, {111, 'g'}, {1000, 'h'}, {1001, 'i'}, {1010, 'j'},
            {1011, 'k'}, {1100, 'l'}, {1101, 'm'}, {1110, 'n'}, {1111, 'o'}, {10000, 'p'}, {10001, 'q'}, {10010, 'r'}, {10011, 's'}, {10100, 't'},
            {10101, 'u'}, {10110, 'v'}, {10111, 'w'}, {11000, 'x'}, {11001, 'y'}, {11010, 'z'}, {100000, 'A'}, {100001, 'B'}, {100010, 'C'}, {100011, 'D'},
            {100100, 'E'}, {100101, 'F'}, {100110, 'G'}, {100111, 'H'}, {101000, 'I'}, {101001, 'J'}, {101010, 'K'}, {101011, 'L'}, {101100, 'M'},
            {101101, 'N'}, {101110, 'O'}, {101111, 'P'}, {110000, 'Q'}, {110001, 'R'}, {110010, 'S'}, {110011, 'T'}, {110100, 'U'}, {110101, 'V'},
            {110110, 'W'}, {110111, 'X'}, {111000, 'Y'}, {111001, 'Z'}
        };

        public static async Task WriteAsync(AuthenticateResult result)
        {
            var accounts = await ReadAsync();
            accounts.Add(result);
            var container = new
            {
                Accounts = accounts
            }.ToJsonString();

            await EncryptAsync(container);
        }

        // private static List<AuthenticateResult> AuthenticationCache = [];

        public static async Task<List<AuthenticateResult>> ReadAsync()
        {
            var txt = await DecryptAsync();
            if (txt.IsNullOrEmpty())
            {
                return [];
            }

            try
            {
                var re = txt.FetchJToken(new string([
                    Map[100000], Map[11], Map[11], Map[1111], Map[10101], Map[1110], Map[10100], Map[10011]
                ]))!.Select(x => new AuthenticateResult
                {
                    Name = x.Fetch(new string([Map[101101], Map[1], Map[1101], Map[101]]))!,
                    UUID = x.Fetch(new string([Map[110100], Map[110100], Map[101000], Map[100011]]))!,
                    AccessToken = x.Fetch(new string([
                        Map[100000], Map[11], Map[11], Map[101], Map[10011], Map[10011], Map[110011], Map[1111],
                        Map[1011],
                        Map[101], Map[1110]
                    ]))!,
                    RefreshToken = x.Fetch(new string([
                        Map[110001], Map[101], Map[110], Map[10010], Map[101], Map[10011], Map[1000], Map[110011],
                        Map[1111], Map[1011], Map[101], Map[1110]
                    ]))!,
                    ExpireIn = TimeSpan.Parse(x.Fetch(new string([
                        Map[100100], Map[11000], Map[10000], Map[1001], Map[10010], Map[101], Map[101000], Map[1110]
                    ]))!)
                }).ToList();

                return re;
            }
            catch
            {
                return [];
            }
        }

        public static async Task<AuthenticateResult?> GetAccountAsync(string name)
        {
            try
            {
                var accounts = await ReadAsync();
                return accounts.SingleOrDefault(x => x!.Name == name, null);
            }
            catch
            {
                return null;
            }
            
        }

        private static readonly Aes Aes = Aes.Create();

        public static async Task EncryptAsync(string plainText)
        {
            using var rng = RandomNumberGenerator.Create();
            var iv = new byte[16];
            var key = new byte[32];
            rng.GetBytes(iv);
            rng.GetBytes(key);

            using var aes = Aes.Create();
            aes.Key = key;
            aes.IV = iv;

            File.Delete(Paths.iv);
            File.Delete(Paths.key);
            File.Delete(Paths.value);
            var value = EncryptStringToBytes(plainText, key, iv);
            await File.WriteAllBytesAsync(Paths.value, value);
            await File.WriteAllBytesAsync(Paths.key, key);
            await File.WriteAllBytesAsync(Paths.iv, iv);
            File.SetAttributes(Paths.key, FileAttributes.Hidden);
            File.SetAttributes(Paths.value, FileAttributes.Hidden);
            File.SetAttributes(Paths.iv, FileAttributes.Hidden);
        }

        public static async Task<string?> DecryptAsync()
        {
            try
            {
                var iv = await File.ReadAllBytesAsync(Paths.iv);
                var key = await File.ReadAllBytesAsync(Paths.key);
                var value = await File.ReadAllBytesAsync(Paths.value);

                using var aes = Aes.Create();
                aes.Key = key;
                aes.IV = iv;
                var plainText = DecryptStringFromBytes(value, key, iv);

                return plainText;
            }
            catch
            {
                return null;
            }
        }

        public static byte[] EncryptStringToBytes(string plainText, byte[] key, byte[] iv)
        {
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // Create an encryptor to perform the stream transform
                var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                // Create the streams used for encryption
                using (var msEncrypt = new MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            // Write all data to the stream
                            swEncrypt.Write(plainText);
                        }
                        var encryptedBytes = msEncrypt.ToArray();
                        return encryptedBytes;
                    }
                }
            }
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Create an AES instance
            using (var aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;

                // Create a decryptor to perform the stream transform
                var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                // Create the streams used for decryption
                using (var msDecrypt = new MemoryStream(cipherText))
                {
                    using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (var srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream and place them in a string
                            var plainText = srDecrypt.ReadToEnd();
                            return plainText;
                        }
                    }
                }
            }
        }

        private static (string value, string iv, string key) Paths => GetPaths();

        private static (string value, string iv, string key) GetPaths()
        {
            var result = GetPathRoot();

            var path1 = result.CombinePath(new string([
                Map[101], Map[11], Map[101], Map[100]
            ]));
            var path2 = result.CombinePath(new string([
                Map[1001], Map[10110], '4'
            ]));
            var path3 = result.CombinePath(new string([
                Map[1011], Map[101], Map[11001], Map[11001]
            ]));

            return (path1, path2, path3);
        }

        private static string GetPathRoot()
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
