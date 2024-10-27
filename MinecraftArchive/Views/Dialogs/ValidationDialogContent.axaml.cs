using Avalonia.Controls;
using MinecraftArchive.ViewModels.Dialogs;

namespace MinecraftArchive.Views.Dialogs {
    public partial class ValidationDialogContent : UserControl {
        public static ValidationDialogContentViewModel? ViewModel { get; private set; }

        public ValidationDialogContent() {
            InitializeComponent();
            DataContext = ViewModel = new();
        }
    }
}
