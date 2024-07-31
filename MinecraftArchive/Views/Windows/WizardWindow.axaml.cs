using Avalonia.Controls;
using System.Diagnostics;
using System.Threading.Tasks;
using MinecraftArchive.Class.Utils;

namespace MinecraftArchive.Views.Windows
{
    public partial class WizardWindow : Window
    {
        public WizardWindow()
        {
            Initialized += WizardWindow_Initialized;
            InitializeComponent();
        }

        private async void WizardWindow_Initialized(object? sender, System.EventArgs e) {       
            await Task.Delay(1000);
            TopBar.Spacing = 15;
        }
    }
}
