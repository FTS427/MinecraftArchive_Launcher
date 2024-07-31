using Avalonia.Controls;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Enum;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages {
    public partial class NetConfigPage : UserControl {   
        public static NetConfigPageViewModel ViewModel { get; set; } = new();
        public NetConfigPage()
        {
            InitializeComponent();
            DataContext = ViewModel;

            if (GlobalResources.LauncherData.CurrentDownloadAPI is DownloadApiType.Bmcl)
            {
                bmcl.IsChecked = true;
            }
            else if (GlobalResources.LauncherData.CurrentDownloadAPI is DownloadApiType.Mojang)
            {
                official.IsChecked = true;
            }
        }
    }
}
