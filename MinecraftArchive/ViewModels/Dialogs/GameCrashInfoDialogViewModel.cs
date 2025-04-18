﻿using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Launch;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Utils;

namespace MinecraftArchive.ViewModels.Dialogs {
    public class GameCrashInfoDialogViewModel : ReactiveObject {
        public GameCrashInfoDialogViewModel() {
            if (!GlobalResources.LaunchInfoData.IsNull()) {
                Memory = GlobalResources.LaunchInfoData.MaxMemory;
            }
        }

        [Reactive]
        public string CrashInfo { get; set; }

        [Reactive]
        public GameCore GameCore { get; set; }

        [Reactive]
        public Account Account { get; set; } = Account.Default;

        [Reactive]
        public int Memory { get; set; }

        [Reactive]
        public int JavaVersion { get; set; }

        [Reactive]
        public string OsPlatform { get; set; } = SystemUtils.GetPlatformName();

        [Reactive]
        public ObservableCollection<string> CrashModpacks { get; set; }

        public void HideDialogAction() {
        }
    }
}
