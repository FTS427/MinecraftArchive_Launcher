using ReactiveUI.Fody.Helpers;
using System.Collections.ObjectModel;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.Views.Pages;
using System.Threading.Tasks;
using System.ComponentModel;
using MinecraftArchive.Class.AppData;
using DialogHostAvalonia;
using MinecraftArchive.Views.Dialogs;

namespace MinecraftArchive.ViewModels.Pages {
    public class AccountPageViewModel : ViewModelBase {
        public AccountPageViewModel() {
            Init();
        }

        [Reactive]
        public ObservableCollection<AccountViewData> GameAccounts { get; set; } = new();

        [Reactive]
        public AccountViewData CurrentGameAccount { get; set; } = null;

        [Reactive]
        public bool IsSelectAccount { get; set; }

        public async void CreateAccountAction() {
            await DialogHost.Show(new ValidationDialogContent(), "dialogHost");
        }

        public async void Init() {
            PropertyChanged += OnPropertyChanged;            
            await Task.Run(() => {
                GameAccounts = CacheResources.Accounts;
            });
        }

        public void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(CurrentGameAccount)) {
                IsSelectAccount = !CurrentGameAccount.IsNull();
            }
        }

        public override void GoBackAction() {
            new ActionCenterPage().Navigation();
        }
    }
}
