using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherMinecraftLauncher.Models.Attributes
{
    internal class MinecraftDirectoryAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            return value is string path && Directory.Exists(path);
        }
    }
}
