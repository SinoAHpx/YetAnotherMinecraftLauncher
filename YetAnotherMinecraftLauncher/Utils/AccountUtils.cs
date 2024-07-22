using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleLauncher.NET.Models.Authentication;
using YetAnotherMinecraftLauncher.Views.Controls;

namespace YetAnotherMinecraftLauncher.Utils
{
    public static class AccountUtils
    {
        //todo: this is not accomplished
        public static AuthenticateResult ToAuthenticateResult(this SelectiveItem item)
        {
            return item.Title;
        }
    }
}
