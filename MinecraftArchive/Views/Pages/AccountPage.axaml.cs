using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Threading;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages {
    public partial class AccountPage : UserControl {
        public static AccountPageViewModel ViewModel { get; private set; } = new();

        public AccountPage() {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private async void OnAccountDeleteClick(object? sender, RoutedEventArgs e) {
            try {
                var userInfo = ((AccountViewData)(sender as MenuItem)!.DataContext!).Data;
                await AccountUtils.DeleteAsync(userInfo);
                ViewModel.GameAccounts = CacheResources.Accounts;
            }
            catch (Exception ex) {
                $"{ex}".ShowInfoDialog("�����������쳣");
            }
        }
    }
}
