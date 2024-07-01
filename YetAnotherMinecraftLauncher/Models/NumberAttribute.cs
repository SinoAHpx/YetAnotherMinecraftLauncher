using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Manganese.Text;

namespace YetAnotherMinecraftLauncher.Models
{
    internal class NumberAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            var str = value?.ToString();
            return str is not null && str.IsInteger();
        }
    }
}
