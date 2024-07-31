using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using System.Collections.ObjectModel;
using MinecraftArchive.control.Controls.Bar;

namespace MinecraftArchive.control {
    public partial class App : Application {
        public static ObservableCollection<MessageTipsBar> Cache { get; private set; } = new();

        public override void Initialize() {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted() {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop) {
                desktop.MainWindow = new MainWindow();
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}