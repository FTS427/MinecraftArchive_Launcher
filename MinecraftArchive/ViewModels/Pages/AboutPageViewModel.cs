using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Views.Pages;

namespace MinecraftArchive.ViewModels.Pages {
    public class AboutPageViewModel : ViewModelBase {
        public override void GoBackAction() {
            new SelectConfigPage().Navigation();
        }
    }
}
