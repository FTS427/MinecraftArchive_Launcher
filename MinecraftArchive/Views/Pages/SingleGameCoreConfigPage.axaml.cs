using Avalonia.Controls;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages {
    public partial class SingleGameCoreConfigPage : UserControl {
        public GameCoreViewData ViewData { get; set; } = null;
        public SingleGameCoreConfigPageViewModel ViewModel { get; set; }
        public SingleGameCoreConfigPage() {
            InitializeComponent();
        }

        public SingleGameCoreConfigPage(GameCoreViewData data) {
            InitializeComponent();
            ViewData = data;
            DataContext = ViewModel = new(data);
        }
    }
}
