using Manganese.Array;
using Manganese.Process;
using ModuleLauncher.NET.Authentications;

namespace YetAnotherMinecraftLauncher.Debugy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ms = new MicrosoftAuthenticator();
            Console.WriteLine(ms.LoginUrl);
            ms.LoginUrl.OpenUrl();
        }
    }
}
