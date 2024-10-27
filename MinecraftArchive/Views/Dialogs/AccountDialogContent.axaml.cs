using Avalonia.Controls;
using MinecraftArchive.ViewModels.Dialogs;

namespace MinecraftArchive.Views.Dialogs {
    public partial class AccountDialogContent : UserControl {
        public static AccountDialogContentViewModel? ViewModel { get; private set; }

        public AccountDialogContent() {
            InitializeComponent();
            DataContext = ViewModel = new();
        }
    }
}
