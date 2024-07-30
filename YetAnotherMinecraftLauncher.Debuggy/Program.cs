using System.Security.Cryptography;
using System.Text;
using ModuleLauncher.NET.Models.Launcher;
using Manganese.Text;
using Console = System.Console;
using System.Reflection;
using YetAnotherMinecraftLauncher.Utils;

namespace YetAnotherMinecraftLauncher.Debugy
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var name = "Shit";
            var container = new
            {
                name = "AHx"
            };

            Console.WriteLine(container.ToJsonString());
        }

        private static string Match(string origin)
        {
            var cr = origin.ToCharArray();
            var str = "";
            foreach (var c in cr)
            {
                str += $"Map[{Map.Single(x => x.Value == c).Key}],";
            }

            return $"new string([{str.TrimEnd(',')}])";
        }

        private static readonly Dictionary<int, char> Map = new()
        {
            {1, 'a'}, {10, 'b'}, {11, 'c'}, {100, 'd'}, {101, 'e'}, {110, 'f'}, {111, 'g'}, {1000, 'h'}, {1001, 'i'}, {1010, 'j'},
            {1011, 'k'}, {1100, 'l'}, {1101, 'm'}, {1110, 'n'}, {1111, 'o'}, {10000, 'p'}, {10001, 'q'}, {10010, 'r'}, {10011, 's'}, {10100, 't'},
            {10101, 'u'}, {10110, 'v'}, {10111, 'w'}, {11000, 'x'}, {11001, 'y'}, {11010, 'z'}, {100000, 'A'}, {100001, 'B'}, {100010, 'C'}, {100011, 'D'},
            {100100, 'E'}, {100101, 'F'}, {100110, 'G'}, {100111, 'H'}, {101000, 'I'}, {101001, 'J'}, {101010, 'K'}, {101011, 'L'}, {101100, 'M'},
            {101101, 'N'}, {101110, 'O'}, {101111, 'P'}, {110000, 'Q'}, {110001, 'R'}, {110010, 'S'}, {110011, 'T'}, {110100, 'U'}, {110101, 'V'},
            {110110, 'W'}, {110111, 'X'}, {111000, 'Y'}, {111001, 'Z'}
        };

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
