using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using Manganese.Text;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using ModuleLauncher.NET.Models.Launcher;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models;
using YetAnotherMinecraftLauncher.Utils;

namespace YetAnotherMinecraftLauncher.ViewModels
{
    internal class SettingsViewModel : ViewModelBase
    {
        public ReactiveCommand<Unit, Unit> ReturnCommand { get; set; }

        #region Minecraft Settings

        private ObservableCollection<MinecraftJava> _javaExecutables = [];

        public ObservableCollection<MinecraftJava> JavaExecutables
        {
            get => _javaExecutables;
            set => this.RaiseAndSetIfChanged(ref _javaExecutables, value);
        }
        public ReactiveCommand<int, Unit> RemoveJavaCommand { get; set; }

        public ReactiveCommand<Unit, Unit> BrowseJavaCommand { get; set; }


        private string _allocatedMemorySize;

        [Number]
        public string AllocatedMemorySize
        {
            get => _allocatedMemorySize;
            set => this.RaiseAndSetIfChanged(ref _allocatedMemorySize, value);
        }

        public ReactiveCommand<Unit, Unit> AutoMemoryCommand { get; set; }

        private string _windowHeight = "480";

        [Number]
        public string WindowHeight
        {
            get => _windowHeight;
            set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
        }

        private string _windowWidth = "854";


        [Number]
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

        #region Launcher Settings

        private List<string> _colors =
        [
            "Red",
            "Pink",
            "Purple",
            "DeepPurple",
            "Indigo",
            "Blue",
            "LightBlue",
            "Cyan",
            "Teal",
            "Green",
            "LightGreen",
            "Lime",
            "Yellow",
            "Amber",
            "Orange",
            "Brown"
        ];

        public List<string> Colors
        {
            get => _colors;
            set => this.RaiseAndSetIfChanged(ref _colors, value);
        }

        private int _colorIndex;

        [Number]
        public int ColorIndex
        {
            get => _colorIndex;
            set => this.RaiseAndSetIfChanged(ref _colorIndex, value);
        }

        private int _afterLaunchAction = 0;

        [Number]
        public int AfterLaunchAction
        {
            get => _afterLaunchAction;
            set => this.RaiseAndSetIfChanged(ref _afterLaunchAction, value);
        }

        private bool _isDarkTheme = true;

        public bool IsDarkTheme
        {
            get => _isDarkTheme;
            set => this.RaiseAndSetIfChanged(ref _isDarkTheme, value);
        }

        private string _customMinecraftDirectory;

        public string CustomMinecraftDirectory
        {
            get => _customMinecraftDirectory;
            set => this.RaiseAndSetIfChanged(ref _customMinecraftDirectory, value);
        }

        public ReactiveCommand<Unit, Unit> BrowseMinecraftDirectoryCommand { get; set; }

        #endregion

        #region Logic

        //ReturnCommand
        public void ReturnToHome()
        {
            MessageBus.Current.SendMessage(nameof(ReturnToHome));
        }

        public async void BrowseJava()
        {
            var storageProvider = TopLevel.GetTopLevel(LifetimeUtils.GetMainWindow())!.StorageProvider;
            var pickResult = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Browse java executable file",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Java executable file")
                    {
                        Patterns = ["javaw.exe","java.exe","javaw","java"]
                    }
                }
                
            });
            if (pickResult.Count < 1)
            {
                return;
            }
            var javaPath = pickResult.Single().Path.LocalPath;

            JavaExecutables.Add(new MinecraftJava()
            {
                Executable = new FileInfo(javaPath),
            });
        }

        public void RemoveJava(int index)
        {
            JavaExecutables.RemoveAt(index);
        }

        public void AutoMemory()
        {

        }


        private void BrowseMinecraftDirectory()
        {

        }

        #endregion

        public SettingsViewModel()
        {
            #region Register commands

            ReturnCommand = ReactiveCommand.Create(ReturnToHome);
            RemoveJavaCommand = ReactiveCommand.Create<int>(RemoveJava);
            BrowseJavaCommand = ReactiveCommand.Create(BrowseJava);
            AutoMemoryCommand = ReactiveCommand.Create(AutoMemory);
            BrowseMinecraftDirectoryCommand = ReactiveCommand.Create(BrowseMinecraftDirectory);

            #endregion
            
            #region Theme changers

            var theme = Application.Current!.LocateMaterialTheme<MaterialTheme>();
            this.WhenAnyValue(v => v.ColorIndex).Subscribe(s =>
            {
                theme.PrimaryColor = Colors[s].MapPrimaryColor();
            });

            this.WhenAnyValue(v => v.IsDarkTheme).Subscribe(i =>
            {
                theme.BaseTheme = i ? BaseThemeMode.Dark : BaseThemeMode.Light;
            });

            #endregion

            #region Configuration

            var configText = ConfigUtils.ReadConfig();
            if (configText is not null)
            {
                WindowWidth = configText.Fetch("WindowWidth");
                WindowHeight = configText.Fetch("WindowHeight");
                IsFullscreen = bool.Parse(configText.Fetch("IsFullscreen")!);
                JavaExecutables = new ObservableCollection<MinecraftJava>(configText.FetchJToken("Javas").Select(x =>
                    new MinecraftJava
                    {
                        Executable = new FileInfo(x.Fetch("ExecutableFile")),
                        Version = 0
                    }));
                AllocatedMemorySize = configText.Fetch("MemorySize");
                DirectlyJoinServer = configText.Fetch("DirectlyJoinServer");

                //todo: replace with the latest Manganese
                IsDarkTheme = bool.Parse(configText.Fetch("IsDarkTheme"));

                //this should not be any problem since the value comes from a combo box
                ColorIndex = configText.Fetch("Color").ToInt32();
                AfterLaunchAction = configText.Fetch("AfterLaunchAction").ToInt32();
            }

            //we'd have a configurator here, so the code could be more maintainable
            this.WhenAnyValue(x1 => x1.AllocatedMemorySize,
                    x2 => x2.JavaExecutables.Count,
                    x3 => x3.WindowHeight,
                    x4 => x4.WindowWidth,
                    x5 => x5.IsFullscreen,
                    x6 => x6.DirectlyJoinServer,
                    x7 => x7.ColorIndex,
                    x8 => x8.IsDarkTheme,
                    x9 => x9.AfterLaunchAction,
                    (x1, _, x3, x4, x5, x6, x7, x8,x9) => new
                    {
                        MemorySize = x1,
                        Javas = JavaExecutables.Select(x =>
                            new
                            {
                                ExecutableFile = x.Executable?.FullName,
                                x.Version
                            }),
                        WindowHeight = x3,
                        WindowWidth = x4,
                        IsFullscreen = x5,
                        DirectlyJoinServer = x6,
                        Color = x7,
                        IsDarkTheme = x8,
                        AfterLaunchAction = x9
                    })
                .Subscribe(async t =>
                {
                    await t.WriteConfigAsync();
                });

            #endregion
        }

    }
}
