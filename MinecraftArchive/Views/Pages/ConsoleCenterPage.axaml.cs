using Avalonia.Controls;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages {
    public partial class ConsoleCenterPage : UserControl {
        public static ConsoleCenterPageViewModel? ViewModel { get; set; }
        public ConsoleCenterPage() {
            InitializeComponent();
            DataContext = ViewModel = new();
        }
    }
}
