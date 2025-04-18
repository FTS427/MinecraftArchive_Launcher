﻿using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Utilities;
using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using MinecraftArchive.Class.Utils;

namespace MinecraftArchive.ViewModels.Pages {
    public class ResourePackConfigPageVM : ViewModelBase {
        public ResourePackConfigPageVM(GameCore core) {
            PropertyChanged += OnPropertyChanged;

            Current = core;
        }

        private async void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName == nameof(Current)) {
                Toolkit = new(Current, true);
                var result = await Task.Run(async () => await Toolkit.LoadAllAsync());

                if (result.Any()) {
                    await Task.Run(() => ResourcePacks = result.ToObservableCollection());
                } else HasResourcePack = 1;

                Trace.WriteLine($"[Info] 共有 {ResourcePacks.Count} 个资源包");
            }
        }

        [Reactive]
        public GameCore Current { get; set; }

        [Reactive]
        public int HasResourcePack { get; set; } = 0;

        [Reactive]
        public ObservableCollection<ResourcePack> ResourcePacks { get; set; } = new();

        public static ResourcePackUtil? Toolkit { get; set; }
    }
}
