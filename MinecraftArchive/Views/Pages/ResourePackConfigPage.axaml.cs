using Avalonia.Controls;
using MinecraftLaunch.Launch;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages
{
    public partial class ResourePackConfigPage : UserControl
    {
        public static ResourePackConfigPageVM? ViewModel { get; set; }
        public ResourePackConfigPage()
        {
            InitializeComponent();
        }

        public ResourePackConfigPage(GameCore core)
        {
            InitializeComponent();
            DataContext = ViewModel = new(core);
        }
    }
}
