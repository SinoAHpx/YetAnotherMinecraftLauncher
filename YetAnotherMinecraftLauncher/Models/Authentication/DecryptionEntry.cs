using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherMinecraftLauncher.Models.Authentication
{
    public class DecryptionEntry
    {
        public byte[] IV { get; set; }

        public string Value { get; set; }
    }
}
