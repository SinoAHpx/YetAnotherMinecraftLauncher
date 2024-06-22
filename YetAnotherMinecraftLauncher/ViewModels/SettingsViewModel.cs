using System.Collections.ObjectModel;
using System.Reactive;
using ModuleLauncher.NET.Models.Launcher;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Views;

namespace YetAnotherMinecraftLauncher.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> ReturnActionCommand { get; set; }

        #region Settings

        private ObservableCollection<MinecraftJava>? _javaExecutables;

        public ObservableCollection<MinecraftJava>? JavaExecutables
        {
            get => _javaExecutables;
            set => this.RaiseAndSetIfChanged(ref _javaExecutables, value);
        }
        public ReactiveCommand<int, Unit> RemoveJavaCommand { get; set; }

        public ReactiveCommand<Unit, Unit> BrowseJavaCommand { get; set; }


        private string _allocatedMemorySize;

        public string AllocatedMemorySize
        {
            get => _allocatedMemorySize;
            set => this.RaiseAndSetIfChanged(ref _allocatedMemorySize, value);
        }

        public ReactiveCommand<Unit, Unit> AutoMemoryCommand { get; set; }

        private string _windowHeight = "480";

        public string WindowHeight
        {
            get => _windowHeight;
            set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
        }

        private string _windowWidth = "854";

        public string WindowWidth
        {
            get => _windowWidth;
            set => this.RaiseAndSetIfChanged(ref _windowWidth, value);
        }

        private bool _isFullscreen;

        public bool IsFullscreen
        {
            get => _isFullscreen;
            set => this.RaiseAndSetIfChanged(ref _isFullscreen, value);
        }

        private string _directlyJoinServer;

        public string DirectlyJoinServer
        {
            get => _directlyJoinServer;
            set => this.RaiseAndSetIfChanged(ref _directlyJoinServer, value);
        }

        #endregion

        #region Logic

        //ReturnActionCommand
        public void ReturnToHome()
        {
            MessageBus.Current.SendMessage(nameof(ReturnToHome));
        }

        public void BrowseJava()
        {

        }

        public void RemoveJava(int index)
        {
            JavaExecutables?.RemoveAt(index);
        }

        public void AutoMemory()
        {

        }

        #endregion

        public SettingsViewModel()
        {
            #region Register commands

            ReturnActionCommand = ReactiveCommand.Create(ReturnToHome);
            RemoveJavaCommand = ReactiveCommand.Create<int>(RemoveJava);
            BrowseJavaCommand = ReactiveCommand.Create(BrowseJava);
            AutoMemoryCommand = ReactiveCommand.Create(AutoMemory);

            #endregion
        }
    }
}
