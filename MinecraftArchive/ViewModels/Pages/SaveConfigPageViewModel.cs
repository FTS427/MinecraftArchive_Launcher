using MinecraftLaunch.Launch;
using MinecraftLaunch.Utilities;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Utils;

namespace MinecraftArchive.ViewModels.Pages {
    public class SaveConfigPageViewModel : ViewModelBase {
        private GameCore Core { get; set; }

        public SaveConfigPageViewModel(GameCore core) {
            _ = GetSavesAsync(core);
        }
        
        public async ValueTask GetSavesAsync(GameCore core) {
            SavesUtil toolkit = new(GlobalResources.LaunchInfoData.GameDirectoryPath);
            var result = await toolkit.LoadAllAsync(core);

            if (result.HasValue()) {

            }
        }
    }
}
