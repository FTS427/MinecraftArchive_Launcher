﻿using Avalonia.Controls;
using MinecraftLaunch.Launch;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.Views.Pages;
using MinecraftArchive.Views.Windows;

namespace MinecraftArchive.ViewModels.Pages {
    public class ConsoleCenterPageViewModel : ViewModelBase {
        public ConsoleCenterPageViewModel() {
            PropertyChanged += OnPropertyChanged;
            GameProcesses = CacheResources.GameProcesses;
        }

        [Reactive]
        public ConsolePage CurrentPage { get; set; }

        [Reactive]
        public bool IsSelectGameProcess { get; set; } = false;

        [Reactive]
        public MinecraftProcessViewData CurrentGameProcess { get; set; } = null;

        [Reactive]
        public ObservableCollection<MinecraftProcessViewData> GameProcesses { get; set; } = new();

        public void StopMinecraftAction() {
            if (!CurrentGameProcess.IsNull()) {
                CurrentGameProcess.MinecraftStopAction();
            }
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(CurrentGameProcess)) {
                CurrentPage = new(CurrentGameProcess);
                IsSelectGameProcess = !CurrentGameProcess.IsNull();
            }
        }
    }
}
