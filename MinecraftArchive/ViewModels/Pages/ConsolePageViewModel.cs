﻿using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Documents;
using Avalonia.Threading;
using DynamicData;
using MinecraftLaunch.Modules.Analyzers;
using MinecraftLaunch.Base.Interfaces;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Class.ViewData;

namespace MinecraftArchive.ViewModels.Pages {
    public class ConsolePageViewModel : ViewModelBase {
        public ConsolePageViewModel(MinecraftProcessViewData data, ScrollViewer box) {
            if (!data.IsNull()) {
                GameLogs.AddRange(data!.Outputs);
                data.Data.ProcessOutput += OnProcessOutput;
            }

            Box = box;
        }

        private void OnProcessOutput(object? sender, IProcessOutput e) {
            e.Raw.ShowLog();
            var result = GameLogAnalyzer.AnalyseAsync(e.Raw);
            Dispatcher.UIThread.Post(() => {
                GameLogs.Add(InlineUtils.CraftGameLogsInline(result));
                Box.ScrollToEnd();
            });
        }

        public ScrollViewer Box { get; private set; }

        [Reactive]
        public AvaloniaList<InlineCollection> GameLogs { get; set; } = new();
    }
}
