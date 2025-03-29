using Avalonia.Controls;
using Avalonia.Media.Imaging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Enum;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Views.Pages;
using MinecraftLaunch.Utilities;
using MinecraftLaunch.Components.Downloader;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Threading;
using System;
using MinecraftArchive.Views.Dialogs;
using DialogHostAvalonia;

namespace MinecraftArchive.ViewModels.Windows {
    public class MainWindowViewModel : ViewModelBase {
        public MainWindowViewModel() {
            this.PropertyChanged += OnPropertyChanged;

            ThreadPool.QueueUserWorkItem(async x => {
                var launcherData = GlobalResources.LauncherData;

                IsLoadImageBackground = launcherData.BakgroundType is "图片";
                IsLoadColorBackground = launcherData.BakgroundType is "主题色";
                IsLoadAcrylicBackground = launcherData.BakgroundType is "亚克力";
                string imagePath = launcherData.ImagePath;
                
                if (IsLoadImageBackground && imagePath.IsFile()) {
                    ImageSource = new Bitmap(imagePath);
                }

                Dispatcher.UIThread.Post(() => {
                    List<WindowTransparencyLevel> effectBackground = new();
                    if (IsLoadAcrylicBackground) {
                        effectBackground.Add(WindowTransparencyLevel.AcrylicBlur);
                        App.CurrentWindow.TransparencyLevelHint = effectBackground;
                    }

                    bool isLoadMica = launcherData.BakgroundType.Contains("云母");
                    if (isLoadMica) {
                        effectBackground.Add(WindowTransparencyLevel.Mica);
                        App.CurrentWindow.TransparencyLevelHint = effectBackground;
                    }
                });

                APIManager.Current = launcherData.CurrentDownloadAPI switch {
                    DownloadApiType.Bmcl => APIManager.Bmcl,
                    DownloadApiType.Mojang => APIManager.Mojang,
                    _ => APIManager.Mcbbs,
                };
            });
        }

        [Reactive]
        public Bitmap ImageSource { get; set; }

        [Reactive]
        public UserControl CurrentPage { get; set; } = new HomePage();

        [Reactive]
        public string NotificationCountText { get; set; } = "通知中心";

        [Reactive]
        public bool HasNotification { get; set; } = false;

        [Reactive]
        public bool IsLoadImageBackground { get; set; } = false;

        [Reactive]
        public bool IsLoadColorBackground { get; set; } = false;
        
        [Reactive]
        public bool IsLoadAcrylicBackground { get; set; } = false;

        public string Version => "1.2.8.4";

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(CurrentPage)) {
                Trace.WriteLine("[Info] 活动页面已改变");
            }
        }
    }
}
