using Avalonia.Controls;
using MinecraftArchive.ViewModels.Dialogs;

namespace MinecraftArchive.Views.Dialogs
{
    public partial class GameCrashInfoDialog : UserControl {
        public static GameCrashInfoDialogViewModel? ViewModel { get; set; } = new();
        public GameCrashInfoDialog() {       
            InitializeComponent();
            DataContext = ViewModel;
        }
    }
}
