﻿using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using MinecraftLaunch.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text;
using System.Threading.Tasks;
using MinecraftArchive.Class.Models;

namespace MinecraftArchive.Class.AppData {
    public class GlobalResources {
        [SupportedOSPlatform("Linux")]
        public static readonly string[] LinuxJavaHomePaths = { "/usr/lib/jvm", "/usr/lib32/jvm", ".usr/lib64/jvm" };

        public const string CurseforgeToken = "$2a$10$Awb53b9gSOIJJkdV3Zrgp.CyFP.dI13QKbWn/4UZI4G4ff18WneB6";

        public const string ClientId = "9fd44410-8ed7-4eb3-a160-9f1cc62c824c";

        public const string MojangNewsApi = "https://launchercontent.mojang.com/news.json";

        public const string HitokotoApi = "https://v1.hitokoto.cn/";

        public const string LanguageDir = "avares://MinecraftArchive/Assets/Strings/";

        public static CurseForgeUtil CurseForgeToolkit { get; } = new(CurseforgeToken);

        public static LaunchInfoDataModel LaunchInfoData { get; set; } = new();

        public static LauncherDataModel LauncherData { get; set; } = new();

        public static LaunchInfoDataModel DefaultLaunchInfoData { get; } = new();

        public static LauncherDataModel DefaultLauncherData { get; } = new();

        public static ResourceDictionary CurrentLanguage { get; set; } =
            (ResourceDictionary)AvaloniaXamlLoader.Load(new($"avares://MinecraftArchive/Assets/Strings/zh-cn.axaml"));
    }
}
