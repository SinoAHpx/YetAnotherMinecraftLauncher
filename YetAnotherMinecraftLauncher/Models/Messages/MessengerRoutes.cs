using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YetAnotherMinecraftLauncher.Models.Messages
{
    public enum MessengerRoutes
    {
        //navigation
        ReturnToHome,
        ToDownload,

        //behavior
        UpdateVersions,
        UpdateAccounts,

        //response
        RemoveAccount, 
        SelectAccount, 
        RemoveVersion, 
        SelectVersion, 
    }
}
