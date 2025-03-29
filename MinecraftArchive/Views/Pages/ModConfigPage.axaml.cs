using Avalonia.Controls;
using MinecraftLaunch.Launch;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages
{
    public partial class ModConfigPage : UserControl
    {
        public static ModConfigPageViewModel? ViewModel { get; set; }
        public ModConfigPage()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public ModConfigPage(GameCore core)
        {
            InitializeComponent();
            DataContext = ViewModel = new(core);
        }
    }
}
