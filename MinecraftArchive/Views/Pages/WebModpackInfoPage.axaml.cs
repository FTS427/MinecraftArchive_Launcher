using Avalonia.Controls;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages
{
    public partial class WebModpackInfoPage : UserControl
    {
        public static WebModpackInfoPageViewModel? ViewModel { get; set; }
        public WebModpackInfoPage() {       
            InitializeComponent();
            DataContext = ViewModel = new();
        }

        public WebModpackInfoPage(WebModpackViewData data) {
            InitializeComponent();
            DataContext = ViewModel = new(data);
        }
    }
}
