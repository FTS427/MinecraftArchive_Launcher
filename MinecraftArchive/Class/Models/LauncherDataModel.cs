using Avalonia.Media;
using MinecraftLaunch.Components.Downloader;
using System.Text.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MinecraftArchive.Class.Enum;

namespace MinecraftArchive.Class.Models
{
    /// <summary>
    /// 启动器设置数据模型
    /// </summary>
    public class LauncherDataModel {
        [JsonPropertyName("accentColor")]
        public Color AccentColor { get; set; } = Color.FromRgb(255, 185, 0);
        
        [JsonPropertyName("bakgroundType")]
        public string BakgroundType { get; set; } = "主题色";

        [JsonPropertyName("themeType")]
        public string ThemeType { get; set; } = "亮色";

        [JsonPropertyName("parallaxType")]
        public string ParallaxType { get; set; } = "无";

        [JsonPropertyName("imagePath")]
        public string ImagePath { get; set; } = string.Empty;

        [JsonPropertyName("currentdownloadAPI")]
        public DownloadApiType CurrentDownloadAPI { get; set; } = DownloadApiType.Mojang;

        [JsonPropertyName("languageType")]
        public string LanguageType { get; set; } = "zh-cn";

        [JsonPropertyName("downloadCount")]
        public int DownloadCount { get; set; } = 64;
    }
}
