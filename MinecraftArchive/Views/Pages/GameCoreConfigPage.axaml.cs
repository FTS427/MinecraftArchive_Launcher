using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.ViewModels.Pages;

namespace MinecraftArchive.Views.Pages {
    public partial class GameCoreConfigPage : UserControl {   

        public static GameCoreConfigPageViewModel? ViewModel { get; set; }
        public GameCoreConfigPage() {       
            InitializeComponent();
            DataContext = ViewModel;

            foreach (ToggleButton item in ButtonGroup.Children) {
                item!.Click += (x, e) => {
                    foreach (ToggleButton item1 in ButtonGroup.Children) {                   
                        item1.IsChecked = false;
                    }

                    item.IsChecked = true;
                };
            }
        }

        public GameCoreConfigPage(GameCoreViewData core) { 
            InitializeComponent();
            DataContext = ViewModel = new(core);

            foreach (ToggleButton item in ButtonGroup.Children) {           
                item!.Click += (x, e) => {
                    foreach (ToggleButton item1 in ButtonGroup.Children) {                   
                        item1.IsChecked = false;
                    }

                    item.IsChecked = true;
                };
            }
        }
    }
}
