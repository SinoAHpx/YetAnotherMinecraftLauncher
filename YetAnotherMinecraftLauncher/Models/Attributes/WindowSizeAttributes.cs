using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Platform;
using Manganese.Text;
using YetAnotherMinecraftLauncher.Utils;

namespace YetAnotherMinecraftLauncher.Models.Attributes
{
    internal class WindowHeightAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string rawSize)
            {
                if (!rawSize.IsInt32())
                {
                    return false;
                }
                var bounds = LifetimeUtils.GetMainWindow()?.Screens.Primary?.Bounds;

                if (bounds != null)
                {
                    var size = rawSize.ToInt32();
                    return size <= bounds.Value.Height;
                }

                if (rawSize is "" or null)
                {
                    return true;
                }

                return rawSize.IsInt32();


            }
            return false;

        }
    }

    internal class WindowWidthAttribute : ValidationAttribute
    {
        public override bool IsValid(object? value)
        {
            if (value is string rawSize)
            {
                if (!rawSize.IsInt32())
                {
                    return false;
                }

                var bounds = LifetimeUtils.GetMainWindow()?.Screens.Primary?.Bounds;

                if (bounds != null)
                {
                    var size = rawSize.ToInt32();
                    return size <= bounds.Value.Width;
                }
                if (rawSize is "" or null)
                {
                    return true;
                }

                return rawSize.IsInt32();
            }
            return false;

        }
    }
}
