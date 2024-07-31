using MinecraftLaunch.Modules.Models.Launch;
using ReactiveUI.Fody.Helpers;
using System.Diagnostics;
using MinecraftArchive.Class.Models;
using MinecraftArchive.Class.Utils;

namespace MinecraftArchive.Class.ViewData {
    public class GameCoreViewData : ViewDataBase<GameCore> {
        public GameCoreViewData(GameCore data) : base(data) {
            AsyncRunner(data);
        }

        [Reactive]
        public SingleCoreModel SingleConfig { get; set; }

        public async void AsyncRunner(GameCore data) {
            var result = await JsonUtils.ReadSingleGameCoreJsonAsync(data);

            if (!result.IsNull()) {
                SingleConfig = result;
            }
        }

        public void OpenFolderAction() {
            using var process = Process.Start(new ProcessStartInfo(Data.Root.FullName) {
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
