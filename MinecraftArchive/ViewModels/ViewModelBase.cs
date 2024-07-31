using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.control.Animation;
using MinecraftArchive.Views.Pages;
using MinecraftArchive.Views.Windows;

namespace MinecraftArchive.ViewModels {
    public class ViewModelBase : ReactiveObject {
        public Dispatcher Dispatcher => Dispatcher.UIThread;

        public virtual void GoBackAction() {
            App.CurrentWindow.OpenTopBar();
            new HomePage().Navigation();

            if (GlobalResources.LauncherData.BakgroundType is not "亚克力" or "云母(Win11)") {
                OpacityChangeAnimation animation = new(true);
                animation.RunAnimation(App.CurrentWindow.Back);
            }
        }

        public void AccessAndInvoke(Action action) {
            if (Dispatcher.CheckAccess()) {
                action();
            } else {
                Dispatcher.Post(action);
            }
        }

        public void AccessAndInvoke(Action action, DispatcherPriority priority) {
            if (Dispatcher.CheckAccess()) {
                action();
            } else {
                Dispatcher.Post(action, priority);
            }
        }
    }
}
