using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reactive;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using DialogHostAvalonia;
using DynamicData.Binding;
using Manganese.Number;
using Manganese.Process;
using Manganese.Text;
using Material.Styles.Themes;
using Material.Styles.Themes.Base;
using ModuleLauncher.NET.Models.Launcher;
using ReactiveUI;
using YetAnotherMinecraftLauncher.Models.Attributes;
using YetAnotherMinecraftLauncher.Models.Messages;
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

        [WindowHeight]
        public string WindowHeight
        {
            get => _windowHeight;
            set => this.RaiseAndSetIfChanged(ref _windowHeight, value);
        }

        private string _windowWidth = "854";


        [WindowWidth]
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

        [MinecraftDirectory]
        public string CustomMinecraftDirectory
        {
            get => _customMinecraftDirectory;
            set => this.RaiseAndSetIfChanged(ref _customMinecraftDirectory, value);
        }

        public ReactiveCommand<Unit, Unit> BrowseMinecraftDirectoryCommand { get; set; }

        private int _minecraftDirectoryType;

        public int MinecraftDirectoryType
        {
            get => _minecraftDirectoryType;
            set => this.RaiseAndSetIfChanged(ref _minecraftDirectoryType, value);
        }

        #endregion

        #region Logic

        //ReturnCommand
        public void ReturnToHome()
        {
            MessengerRoutes.ReturnToHome.Knock();
        }

        public async void BrowseJava()
        {
            var storageProvider = TopLevel.GetTopLevel(LifetimeUtils.GetMainWindow())!.StorageProvider;
            var pickResult = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions()
            {
                Title = "Browse java executable file",
                AllowMultiple = false,
                FileTypeFilter =
                [
                    new FilePickerFileType("Java executable file")
                    {
                        Patterns = ["javaw.exe", "java.exe", "javaw", "java"]
                    }
                ]
            });
            if (pickResult.Count < 1)
            {
                return;
            }

            var javaPath = pickResult.Single().Path.LocalPath;

           var dialog = new InputDialog()
            {
                Title = "Java version",
                Message =
                    "The TextBox below is the a automatically identified java version, which could be empty, in that case you may specify the version manually otherwise Minecraft won't be able to launch.",
                Input = ConfigUtils.GetJavaVersion(javaPath).ToString() ?? ""
            };

            dialog.ConfirmActionCommand = ReactiveCommand.Create(() =>
            {
                JavaExecutables.Add(new MinecraftJava()
                {
                    Executable = new FileInfo(javaPath),
                    Version = dialog.Input.ToInt32()
                });
                DialogHost.Close(null);
            });
            dialog.CancelActionCommand = ReactiveCommand.Create(() =>
            {
                DialogHost.Close(null);
            });

            await dialog.ShowDialogAsync();
        }

        public void RemoveJava(int index)
        {
            JavaExecutables.RemoveAt(index);
        }

        public void AutoMemory()
        {
            var memorySize = ProcessAtom.GetComputerMemorySize().MegabytesToGigabytes();
            AllocatedMemorySize = memorySize switch
            {
                <= 4 => "512",
                >= 4 and <= 7 => "1024",
                >= 7 and <= 15 => "2048",
                >= 16 and <= 31 => "4096",
                >= 31 and <= 63 => "8192",
                >= 63 and <= 127 => "16384",
                >= 127 and <= 255 => "32768",
                >= 255 and <= 511 => "65536",
                >= 511 => "131072",
                _ => "131072"
            };
        }


        private async void BrowseMinecraftDirectory()
        {
            var storageProvider = TopLevel.GetTopLevel(LifetimeUtils.GetMainWindow())!.StorageProvider;

            var pickResult = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                Title = "Browse java executable file",
                AllowMultiple = false,
            });
            if (pickResult.Count < 1)
            {
                return;
            }

            var mcDir = pickResult.Single().Path.LocalPath;

            CustomMinecraftDirectory = mcDir;
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

            if (!ConfigUtils.ConfigText.IsNullOrEmpty())
            {
                //nvm null
                WindowWidth = ConfigUtils.ReadConfig(nameof(WindowWidth)) ?? "";
                WindowHeight = ConfigUtils.ReadConfig(nameof(WindowHeight)) ?? "";
                AllocatedMemorySize = ConfigUtils.ReadConfig(nameof(AllocatedMemorySize)) ?? "";
                DirectlyJoinServer = ConfigUtils.ReadConfig(nameof(DirectlyJoinServer)) ?? "";

                //no null accepted, usually this would not happen except config corrupted
                CustomMinecraftDirectory = ConfigUtils.ReadConfig(nameof(CustomMinecraftDirectory)) ?? "";
                IsFullscreen = ConfigUtils.ReadConfig(nameof(IsFullscreen))?.ToBool() ?? false;
                var javaJToken = ConfigUtils.ConfigText.FetchJToken(nameof(JavaExecutables));

                JavaExecutables = javaJToken != null
                    ? new ObservableCollection<MinecraftJava>(javaJToken.Select(x =>
                        new MinecraftJava
                        {
                            Executable = new FileInfo(x.Fetch("Executable")),
                            Version = (x.Fetch("Version") ?? "0").ToInt32()
                        }))
                    : [];

                IsDarkTheme = ConfigUtils.ReadConfig(nameof(IsDarkTheme))?.ToBool() ?? false;
                ColorIndex = ConfigUtils.ReadConfig(nameof(ColorIndex))?.ToInt32() ?? 0;
                AfterLaunchAction = ConfigUtils.ReadConfig(nameof(AfterLaunchAction))?.ToInt32() ?? 0;
                MinecraftDirectoryType = ConfigUtils.ReadConfig(nameof(MinecraftDirectoryType))?.ToInt32() ?? 0;
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
                    x10 => x10.CustomMinecraftDirectory,
                    x11 => x11.MinecraftDirectoryType,
                    (x1, _, x3, x4, x5, x6, x7, x8, x9, x10, x11) => new
                    {
                        AllocatedMemorySize = x1,
                        JavaExecutables = JavaExecutables.Select(x =>
                            new
                            {
                                Executable = x.Executable?.FullName,
                                x.Version
                            }),
                        WindowHeight = x3,
                        WindowWidth = x4,
                        IsFullscreen = x5,
                        DirectlyJoinServer = x6,
                        ColorIndex = x7,
                        IsDarkTheme = x8,
                        AfterLaunchAction = x9,
                        CustomMinecraftDirectory = x10,
                        MinecraftDirectoryType = x11,
                    })
                .Subscribe(async t =>
                {
                    await t.WriteConfigAsync();
                });

            this.WhenAnyValue(x => x.CustomMinecraftDirectory,
                    y => y.MinecraftDirectoryType,
                    (x, y) => new { CustomMinecraftDirectory = x, MinecraftDirectoryType = y })
                .Subscribe(x =>
                {
                    //actually we don't 
                    MessengerRoutes.UpdateVersions.Knock();
                });

            #endregion
        }

    }
}
