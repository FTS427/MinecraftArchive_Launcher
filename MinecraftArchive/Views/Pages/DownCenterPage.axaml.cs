using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using MinecraftLaunch.Components.Installer;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Enum;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages {
    public partial class DownCenterPage : UserControl {
        public static DownCenterPageViewModel ViewModel { get; set; } = new();
        public DownCenterPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private async void OpenDialogAction(object? sender, RoutedEventArgs args) {
            ViewModel.SelectGameCore = CacheResources.GameCoreInstallInfo = ((sender as Button)!.DataContext as GameCoreEmtity)!;
            ViewModel.InstallerWidth = App.CurrentWindow.Bounds.Width / 2;
            await Task.Run(async () => {
                await HttpUtils.GetModLoadersFromMcVersionAsync(CacheResources.GameCoreInstallInfo.Id);
            });
        }

        public void GoResourceInfoAction(object? sender, RoutedEventArgs args) {
            var resourceInfo = ((sender as Button)!.DataContext) as WebModpackViewData;
            new WebModpackInfoPage(resourceInfo).Navigation();
        }
    }
}
