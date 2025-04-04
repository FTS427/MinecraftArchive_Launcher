﻿using MinecraftLaunch.Base.Interfaces;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Models;

namespace MinecraftArchive.Class.Utils {
    public static class GameCoreUtils {
        public static async ValueTask<ObservableCollection<GameCore>> GetLocalGameCores(string root) {
            var cores = await Task.Run(() => {
                return new GameCoreUtil(root).GetGameCores();
            });
            
            return cores.ToObservableCollection() ?? new();
        }

        public static async ValueTask<ObservableCollection<GameCore>> SearchGameCoreAsync(string root, string text) {
            var cores = await Task.Run(() => {
                try {
                    return new GameCoreUtil(root).GetGameCores();
                }
                catch { }

                return null;
            });

            return cores is null ? new() : cores.ToObservableCollection();
        }

        public static async ValueTask CompLexGameCoreInstallAsync(string version, string name, Action<string, float> action, IEnumerable<ModsPacksModLoaderModel> modloader) {
            InstallerBase<InstallerResponse> installer = null;

            foreach (var mod in modloader) {
                if (mod.Id.Contains("forge")) {
                    var buildResult = (await ForgeInstaller.GetForgeBuildsOfVersionAsync(version)).AsEnumerable();
                    var result = buildResult.Where(x => mod.Id.Contains(x.ForgeVersion))?.FirstOrDefault();

                    installer = new ForgeInstaller(GlobalResources.LaunchInfoData.GameDirectoryPath, result!, GlobalResources.LaunchInfoData.JavaRuntimePath?.JavaPath!, name);
                }

                if (mod.Id.Contains("fabric")) {
                    var buildResult = (await FabricInstaller.GetFabricBuildsByVersionAsync(version)).AsEnumerable();
                    var result = buildResult.Where(x => mod.Id.Contains(x.Loader.Version))?.FirstOrDefault();

                    installer = new FabricInstaller(GlobalResources.LaunchInfoData.GameDirectoryPath, result, name);
                }
            }

            installer!.ProgressChanged += async (_, x) => {
                var progress = x.Progress * 100;
                action($"{Math.Round(progress, 2)}%", progress);
                await Task.Delay(1000);
            };

            var installResult = await Task.Run(async () => await installer.InstallAsync());
            installResult.Success.ShowLog();
        }

        public static async ValueTask CompLexGameCoreInstallAsync(string name, Action<string, float> action, Dependencies dependencies) {
            InstallerBase<InstallerResponse> installer = null;

            if (GameCoreUtil.GetGameCore(GlobalResources.LaunchInfoData.GameDirectoryPath, name) == null) {
                if (!string.IsNullOrEmpty(dependencies.QuiltLoader)) {
                    var buildResult = (await QuiltInstaller.GetQuiltBuildsByVersionAsync(dependencies.Minecraft)).AsEnumerable();
                    var result = buildResult.Where(x => dependencies.QuiltLoader.Contains(x.Loader.Version))?.FirstOrDefault();

                    installer = new QuiltInstaller(GlobalResources.LaunchInfoData.GameDirectoryPath, result, name);
                } else {
                    var buildResult = (await FabricInstaller.GetFabricBuildsByVersionAsync(dependencies.Minecraft)).AsEnumerable();
                    var result = buildResult.Where(x => dependencies.FabricLoader.Contains(x.Loader.Version))?.FirstOrDefault();

                    installer = new FabricInstaller(GlobalResources.LaunchInfoData.GameDirectoryPath, result, name);
                }
            } else return;

            installer!.ProgressChanged += async (_, x) => {
                var progress = x.Progress * 100;
                action($"{Math.Round(progress, 2)}%", progress);
                await Task.Delay(1000);
            };

            var installResult = await Task.Run(async () => await installer.InstallAsync());
            installResult.Success.ShowLog();
        }

        public static async ValueTask<string> GetTotalSizeAsync(GameCore id) {
            double total = 0;
            foreach (var library in id.LibraryResources!) {
                if (library.Size != 0)
                    total += library.Size;
                else if (library.Size == 0 && library.ToFileInfo().Exists)
                    total += library.ToFileInfo().Length;
            }

            try {
                var assets = await new ResourceInstaller(id).GetAssetResourcesAsync();

                foreach (var asset in assets) {

                    if (asset.Size != 0)
                        total += asset.Size;
                    else if (asset.Size == 0 && asset.ToFileInfo().Exists)
                        total += asset.ToFileInfo().Length;
                }
            }
            catch { }

            return $"{double.Parse(((double)total / (1024 * 1024)).ToString("0.00"))} MB";
        }

        public static DirectoryInfo GetOfficialGameCorePath() {
            if (SystemUtils.IsWindows) {
                return new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".minecraft"));
            } else {
                return new(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), ".minecraft"));
            }
        }

        public static double GetOptimumMemory(bool isVanilla, int modCount = 0) {
            var free = SystemUtils.GetMemoryInfo()
                .Free;
            double cache = 1024;

            if (isVanilla && modCount == 0) { //原版
                cache = (2.5 * 1024) + free / 4;
            } else if (modCount > 0) {
                cache = (3 + modCount / 60) * 1024;
                cache = ((free - cache) / 4) + cache;
                if (cache > free) {
                    cache = free - 100;
                }
            } else {
                cache = (3 * 1024) + free / 4;
            }

            return cache;
        }
    }
}
