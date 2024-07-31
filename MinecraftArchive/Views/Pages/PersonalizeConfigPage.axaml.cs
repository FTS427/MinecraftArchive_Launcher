using Avalonia.Controls;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages {
    public partial class PersonalizeConfigPage : UserControl {
        public static PersonalizeConfigPageViewModel ViewModel { get; set; } = new();
        public PersonalizeConfigPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
