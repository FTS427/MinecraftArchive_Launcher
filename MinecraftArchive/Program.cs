using System;
using System.Linq;
using System.Threading;

using Avalonia;
using Avalonia.Media;
using Avalonia.ReactiveUI;
using MinecraftArchive.Class;
using MinecraftArchive.Class.Models;
using MinecraftArchive.Class.Utils;

namespace MinecraftArchive;

class Program {
    [STAThread]
    public static void Main(string[] args) {
        try {
            var builder = BuildAvaloniaApp();
            builder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex) {
            App.Logger.Error(ex.ToString());
            _ = App.Logger.EncapsulateLogsToFileAsync();
            var model = new ExceptionModel() {
                Message = ex.Message,
                StackTrace = ex.StackTrace,
                Exception = ex.GetType().Name,
            };
        }
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UseSystemFont()
            .LogToTrace()
            .UseReactiveUI()
            .UsePlatformDetect();
}
