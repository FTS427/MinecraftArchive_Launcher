using Avalonia.Controls;
using MinecraftLaunch.Launch;
using System;
using System.Collections.ObjectModel;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.ViewModels.Windows;

namespace MinecraftArchive.Views.Windows
{
    public partial class ConsoleWindow : Window
    {
        public static bool IsOpen { get; set; } = false;
        public static ConsoleWindowViewModel ViewModel { get; set; } = new();

        private static ConsoleWindow Console { get; set; } = null;
        public ConsoleWindow() {       
            InitializeComponent();
            Console = this;

            DataContext = ViewModel = new();
            IsOpen = true;

            Closed += (_, _) => {           
                IsOpen = false;
                Console = null;
            };
        }

        public static void WindowActivate() => Console.Activate();

        private void OnPropertyChanged(object? sender, Avalonia.AvaloniaPropertyChangedEventArgs e) {
            $"{e.Property} - {e.NewValue}".ShowLog();
        }


    }

    public static class ProcessManager {
        public static ObservableCollection<Tuple<string, string>> History { get; set; } = new();

        public static ObservableCollection<MinecraftProcessViewData> GameCoreProcesses { get; set; } = new();
    }
}
