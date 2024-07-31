using Avalonia.Controls;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages
{
    public partial class ConsolePage : UserControl {
        public static ConsolePageViewModel ViewModel { get; set; }

        public ConsolePage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        public ConsolePage(MinecraftProcessViewData data) {       
            InitializeComponent();
            DataContext = ViewModel = new(data, e);
        }
    }
}
