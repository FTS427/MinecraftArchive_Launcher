﻿using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Utilities;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.control;

namespace MinecraftArchive.ViewModels.Pages {
    public class ModConfigPageViewModel : ViewModelBase {
        public ModConfigPageViewModel(GameCore core) {
            PropertyChanged += OnPropertyChanged;

            Current = core;
        }

        private async void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Current)) {
                Toolkit = new(Current, true);
                var result = await Task.Run(async () => await Toolkit.LoadAllAsync());

                if (result.Any()) {
                    ModPacks.Clear();
                    try {
                        foreach (var item in result.Select(x => x.CreateViewData<ModPack, ModPackViewData>())) {
                            ModPacks.Add(item);
                            await Task.Delay(10);
                        }
                    }
                    catch (System.Exception) {                        
                    }
                } else HasModPack = 1;

                Trace.WriteLine($"[Info] 共有 {ModPacks.Count} 个模组");
            }
        }

        [Reactive]
        public GameCore Current { get; set; }

        [Reactive]
        public int HasModPack { get; set; } = 0;

        [Reactive]
        public ObservableCollection<ModPackViewData> ModPacks { get; set; } = new();

        public static ModPackUtil? Toolkit { get; set; }
    }
}
