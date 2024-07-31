using Avalonia.Controls;
using System.Threading.Tasks;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages
{
    public partial class AboutPage : UserControl {   
        public AboutPage() {
            InitializeComponent();
            DataContext = new AboutPageViewModel();
        }
    }
}
