using System.ComponentModel.DataAnnotations;
using Manganese.Text;

namespace YetAnotherMinecraftLauncher.Models.Attributes;

internal class NumberAttribute : ValidationAttribute
{
    public override bool IsValid(object? value)
    {
        var str = value?.ToString();
        return str is not null && str.IsInteger();
    }
}