using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherMinecraftLauncher.Models.Messages
{
    public enum MessageBusRoutes
    {
        //navigation
        ReturnToHome,
        ToDownload,

        //behavior
        UpdateVersions,

        //response
        RemoveAccount, 
        SelectAccount, 
        RemoveVersion, 
        SelectVersion, 
    }
}
