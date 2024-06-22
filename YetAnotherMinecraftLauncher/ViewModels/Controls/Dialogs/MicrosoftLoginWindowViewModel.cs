using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ModuleLauncher.NET.Utilities;
using ReactiveUI;

namespace YetAnotherMinecraftLauncher.ViewModels.Controls.Dialogs
{
    internal class MicrosoftLoginWindowViewModel : ViewModelBase
    {
        private string? _loginUrl;

        public string? LoginUrl
        {
            get => _loginUrl;
            set => this.RaiseAndSetIfChanged(ref _loginUrl, value);
        }

        public MicrosoftLoginWindowViewModel(string loginUrl)
        {
            LoginUrl = loginUrl;
            this.WhenAnyValue(t => t.LoginUrl).Subscribe(s =>
            {
                if(LoginUrl is null)
                    return;
                if (LoginUrl.Contains("code="))
                {
                    var code = LoginUrl.ExtractCode();
                    MessageBus.Current.SendMessage(code, nameof(MicrosoftLoginWindowViewModel));
                }
            });
        }
    }
}
