﻿using Avalonia.Controls.Documents;
using Avalonia.Threading;
using DynamicData;
using MinecraftLaunch.Modules.Analyzers;
using MinecraftLaunch.Base.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.Views.Pages;
using MinecraftArchive.Views.Windows;

namespace MinecraftArchive.ViewModels.Windows {
    public class ConsoleWindowViewModel : ReactiveObject {
        private bool IsProcessClose = false;

        public ConsoleWindowViewModel() {
            History = ProcessManager.History;
            GameCoreProcesses = ProcessManager.GameCoreProcesses;
            GameCoreProcesses.CollectionChanged += OnCollectionChanged;
            PropertyChanged += OnPropertyChanged;
        }

        private void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e) {
            if (e.Action is NotifyCollectionChangedAction.Remove) {
                IsProcessClose = true;
            }
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(CurrentGameCoreProcess) && !IsProcessClose) {
                CurrentConsolePage = new(CurrentGameCoreProcess!);
            }

            IsProcessClose = false;
        }

        [Reactive]
        public ConsolePage? CurrentConsolePage { get; set; } = new();

        [Reactive]
        public MinecraftProcessViewData? CurrentGameCoreProcess { get; set; }

        [Reactive]
        public ObservableCollection<Tuple<string, string>> History { get; set; } = new();

        [Reactive]
        public ObservableCollection<MinecraftProcessViewData> GameCoreProcesses { get; set; } = new();
    }
}
