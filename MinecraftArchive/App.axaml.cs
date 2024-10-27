using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Linq;
using System.Reflection;
using System;
using System.Threading.Tasks;
using MinecraftArchive.Class;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.control;
using MinecraftArchive.Views.Windows;
using MainWindow = MinecraftArchive.Views.Windows.MainWindow;
using ReactiveUI;
using System.Reactive.Concurrency;
using System.Diagnostics;
using MinecraftArchive.Class.AppData;

namespace MinecraftArchive;

public partial class App : Application {
    public static MainWindow CurrentWindow { get; protected set; } = null!;
    public static Logger? Logger { get; protected set; }

    public override void Initialize() {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = MainWindow.ViewModel = new()
            };

            Manager.Current = CurrentWindow = (desktop.MainWindow as MainWindow)!;
            Logger = Logger.LoadLogger(CurrentWindow);
            Logger.Info($"启动器版本：{AssemblyUtil.Version}");
        }

        base.OnFrameworkInitializationCompleted();
    }
}



