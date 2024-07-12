using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manganese.Text;

namespace YetAnotherMinecraftLauncher.Models.Attributes
{
    internal class MinecraftDirectoryAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string path)
            {
                if (path.IsNullOrEmpty())
                {
                    return true;
                }
                return Directory.Exists(path);
            }

            return false;
        }
    }
}
