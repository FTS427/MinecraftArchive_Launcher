﻿using Avalonia.Platform.Storage;
using Avalonia.Threading;
using DialogHostAvalonia;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Components.Downloader;
using MinecraftLaunch.Base.Enums;
using MinecraftLaunch.Components.Installer;
using MinecraftLaunch.Base.Interfaces;
using MinecraftLaunch.Components.Authenticator;
using MinecraftLaunch.Utilities;
using MinecraftOAuth.Authenticator;
using MinecraftOAuth.Module.Enum;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Enum;
using MinecraftArchive.Class.Exceptions;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.Class.ViewData;
using MinecraftArchive.control;
using MinecraftArchive.control.Animation;
using MinecraftArchive.Views.Dialogs;
using MinecraftArchive.Views.Pages;

namespace MinecraftArchive.ViewModels.Pages {
    public class HomePageViewModel : ViewModelBase {
        public HomePageViewModel() {
            this.PropertyChanged += OnPropertyChanged;
        }

        public bool Isopen { get; set; } = false;

        public Account CurrentAccount { get; set; } = Account.Default;

        [Reactive]
        public string SelectGameCoreId { get; set; }

        [Reactive]
        public string SearchCondition { get; set; }

        [Reactive]
        public double SearchSuccess { get; set; } = 0;

        [Reactive]
        public double HasGameCore { get; set; } = 0;

        [Reactive]
        public double PanelHeight { get; set; } = 0;

        [Reactive]
        public GameCoreViewData SelectGameCore { get; set; }

        [Reactive]
        public ObservableCollection<GameCoreViewData> GameCores { get; set; } = new();

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(SearchCondition)) {
                SeachGameCore(SearchCondition);
            }

            if (e.PropertyName is nameof(SelectGameCore) && SelectGameCore != null) {
                GlobalResources.LaunchInfoData.SelectGameCore = SelectGameCore.Data.Id!;
                SelectGameCoreId = SelectGameCore.Data.Id!;
            }
        }

        public async void SeachGameCore(string text) {
            if (!GameCores.Any()) {
                return;
            }

            GameCores.Clear();
            var result = (await GameCoreUtils.SearchGameCoreAsync(GlobalResources.LaunchInfoData.GameDirectoryPath, text)).Distinct();
            GameCores.Load(result.Select(x => x.CreateViewData<GameCore, GameCoreViewData>()));

            if (!GameCores.Any()) {
                SearchSuccess = 1;
            } else SearchSuccess = 0;
        }

        public async void GetGameCoresAction() {
            GameCores.Clear();
            if (string.IsNullOrEmpty(GlobalResources.LaunchInfoData.GameDirectoryPath)) {
                HasGameCore = 1;
                if (GlobalResources.LaunchInfoData.GameDirectorys.Any()) {
                    "GameDirectoryError1".GetText().ShowMessage("Tip",
                        OpenLaunchConfigAction);
                } else {
                    "GameDirectoryError2".GetText().ShowMessage("Tip",
                        OpenLaunchConfigAction);
                }
                return;
            }

            var cores = await GameCoreUtils.GetLocalGameCores(GlobalResources.LaunchInfoData
                .GameDirectoryPath);

            HasGameCore = cores.Any() ? 0 : 1;
            GameCores.Load(cores.Select(x => x.CreateViewData<GameCore, GameCoreViewData>()));
        }

        public async void SelectAccountAction() {
            try {
                var user = CacheResources.Accounts;

                if (user.Count > 1) {
                    await DialogHost.Show(new AccountDialogContent(), "dialogHost");
                    return;
                } else if (user.Count <= 0) {
                    "未添加任何账户，无法继续启动，点击此处转到账户中心添加游戏帐户"
                        .ShowMessage("提示", async () => {
                            OpenActionCenterAction();
                            await Task.Delay(1000);
                            new AccountPage().Navigation();
                        });
                    return;
                }

                CurrentAccount = user.First()
                    .Data
                    .ToAccount();

                LaunchTaskAction();
            }
            catch (Exception) {

            }
            finally {
                GC.Collect();
            }
        }

        public async void LaunchTaskAction() {
            int modCount = 0;
            JavaInfo javaInfo = null!;
            LaunchConfig config = null!;
            NotificationViewData data = null!;
            MinecraftLaunchResponse launchResponse = null!;
            var gameCore = GameCoreUtil.GetGameCore(GlobalResources.LaunchInfoData
                .GameDirectoryPath, SelectGameCoreId);

            try {
                if (NotificationCenterPage.ViewModel.Notifications.Any(x => x.NotificationType is NotificationType.Install && x.Title.Contains(SelectGameCoreId))) {
                    $"检测到游戏核心 \"{SelectGameCoreId}\" 仍有安装任务正在进行，无法启动！"
                        .ShowMessage();

                    return;
                }

                PreUIProcessingAsync();

                var preTasks = new Task[5] {
                    Task.Run(PreConfigProcessingAsync),
                    Task.Run(DownloadAuthlibAsync),
                    Task.Run(ModpackCheckAsync),
                    Task.Run(AccountRefreshAsync),
                    Task.Run(ResourcesCheckOutAsync),
                };

                foreach (var task in preTasks) {
                    await task;
                }
 
                JavaMinecraftLauncher launcher = new(config, GlobalResources.LaunchInfoData.GameDirectoryPath);
                launchResponse = await Task.Run(async () => {
                    return await launcher.LaunchTaskAsync(GlobalResources.LaunchInfoData.SelectGameCore, x => {
                        x.Item2.ShowLog();
                    });
                });

                PostUIProcessingAsync();
            }
            catch (MethodAbortException) {
                return;
            }

            IEnumerable<string> GetAdvancedArguments() {

                if (!string.IsNullOrEmpty(GlobalResources.LaunchInfoData.JvmArgument)) {
                    yield return GlobalResources.LaunchInfoData.JvmArgument;
                }

                var authlibJvm = GetAuthlibJvmArguments();
                if (!string.IsNullOrEmpty(authlibJvm)) {
                    yield return authlibJvm;
                }

                string GetAuthlibJvmArguments() {
                    if (CurrentAccount.Type == AccountType.Yggdrasil) {
                        var account = CurrentAccount as YggdrasilAccount;
                        return $"-javaagent:{Path.Combine(JsonUtils.DataPath, "authlib-injector.jar")}={account!.YggdrasilServerUrl}";
                    }

                    return string.Empty;
                }
            }

            void PreConfigProcessingAsync() {
                data.Progress = "正在启动 - 0%";
                bool flag = !GlobalResources.LaunchInfoData.IsAutoSelectJava && GlobalResources.LaunchInfoData.JavaRuntimePath.Equals(null);//手动选择 Java 的情况
                javaInfo = flag ? GlobalResources.LaunchInfoData.JavaRuntimePath! : GetCurrentJava();//当选择手动时没有任何问题就手动选择，其他情况一律使用自动选择
                config = new LaunchConfig() {
                    JvmConfig = new() {
                        MaxMemory = GlobalResources.LaunchInfoData.IsAutoGetMemory
                        ? GameCoreUtils.GetOptimumMemory(!gameCore.HasModLoader, modCount).ToInt32()
                        : GlobalResources.LaunchInfoData.MaxMemory,
                        JavaPath = SystemUtils.IsWindows ? javaInfo!.JavaPath.ToJavaw().ToFile() : javaInfo!.JavaPath.ToFile(),
                        AdvancedArguments = GetAdvancedArguments(),
                    },
                    GameWindowConfig = new() {
                        Width = GlobalResources.LaunchInfoData.WindowWidth,
                        Height = GlobalResources.LaunchInfoData.WindowHeight,
                        IsFullscreen = GlobalResources.LaunchInfoData.WindowHeight == 0 && GlobalResources.LaunchInfoData.WindowWidth == 0,
                    },
                    Account = CurrentAccount,
                    IsEnableIndependencyCore = true
                };
            }

            void PreUIProcessingAsync() {
                $"开始尝试启动游戏 \"{SelectGameCoreId}\"，点击此处进入通知中心查看启动进度！".ShowMessage(App.CurrentWindow.NotificationCenter.Open);
                data = new(NotificationType.Launch) {
                    Title = $"游戏 {SelectGameCoreId} 的启动任务"
                };

                data.TimerStart();
                NotificationCenterPage.ViewModel.Notifications.Add(data);
            }

            async void PostUIProcessingAsync() {
                data.ProgressOfBar = 100;
                if (launchResponse.State is LaunchState.Succeess) {
                    data.Progress = $"启动成功 - 100%";
                    $"游戏 \"{GlobalResources.LaunchInfoData.SelectGameCore}\" 已启动成功，总用时 {data.RunTime}".ShowMessage("启动成功");
                    var viewData = launchResponse.CreateViewData<MinecraftLaunchResponse, MinecraftProcessViewData>(CurrentAccount, javaInfo);

                    Dispatcher.UIThread.Post(() => {
                        CacheResources.GameProcesses.Add(viewData);
                        launchResponse.ProcessOutput += ProcessOutput;
                        launchResponse.Process.Exited += (sender, e) => {
                            "Game exit".ShowLog();
                            CacheResources.GameProcesses.Remove(viewData);
                        };
                    }, DispatcherPriority.Background);
                } else {
                    await Dispatcher.UIThread.InvokeAsync(() => {
                        data.Progress = $"启动失败 - 100%";
                        $"游戏 \"{GlobalResources.LaunchInfoData.SelectGameCore}\" 启动失败，详细信息 {launchResponse.Exception}".ShowInfoDialog("程序遭遇了异常");
                    });
                }

                data.TimerStop();
            }

            async void DownloadAuthlibAsync() {
                if (!Path.Combine(JsonUtils.DataPath, "authlib-injector.jar").IsFile() && CurrentAccount.Type is AccountType.Yggdrasil) {
                    data!.Progress = "下载 Authlib-Injector 中";

                    await FileDownloader.DownloadAsync(new() {
                        FileName = "authlib-injector.jar",
                        Directory = new(JsonUtils.DataPath),
                        Url = "https://download.mcbbs.net/mirrors/authlib-injector/artifact/45/authlib-injector-1.1.45.jar"
                    });
                }
            }

            async void ModpackCheckAsync() {
                if (gameCore.GetModsPath().IsDirectory()) {
                    data!.Progress = "开始检查 Mod";

                    var result = await Task.Run(async () => {
                        ModPackUtil toolkit = new(gameCore, true);
                        var modpacks = (await toolkit.LoadAllAsync()).Where(x => x.IsEnabled);
                        modCount = modpacks.Count();
                        return modpacks.GroupBy(i => i.Id).Where(g => g.Count() > 1);
                    });

                    if (result.Count() > 0) {
                        foreach (var item in result) {
                            $"模组 \"{item.ToList().First().FileName}\" 在此文件夹已有另一版本，可能导致游戏无法正常启动，已中止启动操作！"
                                .ShowMessage();

                            throw new MethodAbortException("There are multiple identical mods", true);
                        }
                    }
                }
            }

            async void AccountRefreshAsync() {
                try {
                    data.Progress = "开始刷新账户";
                    if (CurrentAccount.Type == AccountType.Yggdrasil) {
                        var userData = CacheResources.Accounts.Where(x => x.Data.Uuid == CurrentAccount.Uuid.ToString()).First().Data;
                        YggdrasilAuthenticator authenticator = new YggdrasilAuthenticator((CurrentAccount as YggdrasilAccount)!.YggdrasilServerUrl,
                            userData.Email, userData.Password);
                        var result = authenticator.RefreshAsync((CurrentAccount as YggdrasilAccount)!).Result;

                        if (result.IsNull()) {
                            "需重新验证".ShowLog();
                        }

                        CurrentAccount = result;
                        await AccountUtils.RefreshAsync(CacheResources.Accounts.Where(x => x.Data.Uuid == result.Uuid.ToString()).First().Data, result);
                    } else if (CurrentAccount.Type == AccountType.Microsoft) {
                        MicrosoftAuthenticator authenticator = new(AuthType.Refresh) {
                            ClientId = GlobalResources.ClientId,
                            RefreshToken = (CurrentAccount as MicrosoftAccount)!.RefreshToken!
                        };

                        var result = authenticator.AuthAsync(x => data.Progress = $"当前步骤：{x}").Result;
                        CurrentAccount = result;
                        await AccountUtils.RefreshAsync(CacheResources.Accounts.Where(x => x.Data.Uuid == result.Uuid.ToString()).First().Data, result);
                    }
                }
                catch (Exception ex) {
                    $"账户刷新失败，详细信息：{ex}".ShowInfoDialog("程序遭遇了异常");
                    data.TimerStop();
                    return;
                }
            }

            void ResourcesCheckOutAsync() {
                try {
                    double progress = 0d;
                    DispatcherTimer timer = new(DispatcherPriority.Send) {
                        Interval = TimeSpan.FromMilliseconds(500)
                    };

                    timer.Tick += (_, _) => {
                        var value = Math.Round(progress * 100, 2);
                        data.ProgressOfBar = value;
                    };

                    ResourceInstaller installer = new(new GameCoreUtil(GlobalResources.LaunchInfoData.GameDirectoryPath)
                        .GetGameCore(GlobalResources.LaunchInfoData.SelectGameCore));

                    data.Progress = $"开始检查并补全丢失的资源";
                    timer.Start();
                    var result = installer.DownloadAsync((s, f) => {
                        progress = f;
                    });
                }
                catch (Exception) {
                }
            }
        }

        public async void ImportModpacksAction() {
            var result = await DialogUtils.OpenFilePickerAsync(new List<FilePickerFileType>() {
                new("整合包文件") { Patterns = new List<string>() { "*.zip", "*.mrpack" } }
            }, "请选择整合包文件");

            if (result!.IsNull()) {
                return;
            }
            var type = ModpacksUtils.ModpacksTypeAnalysis(result.FullName);
            if (type is ModpacksType.Unknown) {
                "未知整合包类型".ShowMessage();
                return;
            }

            await ModpacksUtils.ModpacksInstallAsync(result.FullName, type);
        }

        public async void OpenActionCenterAction() {
            var back = App.CurrentWindow.Back;
            if (GlobalResources.LauncherData.BakgroundType is not "亚克力" or "云母(Win11)") {
                OpacityChangeAnimation opacity = new(false) {
                    RunValue = 0,
                };

                opacity.RunAnimation(back);
            }

            await Task.Delay(100);
            App.CurrentWindow.CloseTopBar();
            await Task.Delay(100);
            new ActionCenterPage().Navigation();
        }

        public void OpenConsoleAction() {
            var back = App.CurrentWindow.Back;
            if (GlobalResources.LauncherData.BakgroundType is not "亚克力" or "云母(Win11)") {
                OpacityChangeAnimation opacity = new(false) {
                    RunValue = 0,
                };

                opacity.RunAnimation(back);
            }

            App.CurrentWindow.CloseTopBar();
            new ConsoleCenterPage().Navigation();
        }

        public void OpenLaunchConfigAction() {
            var back = App.CurrentWindow.Back;
            if (GlobalResources.LauncherData.BakgroundType is not "亚克力" or "云母(Win11)") {
                OpacityChangeAnimation opacity = new(false) {
                    RunValue = 0,
                };

                opacity.RunAnimation(back);
            }

            App.CurrentWindow.CloseTopBar();
            new LaunchConfigPage().Navigation();
        }

        public JavaInfo GetCurrentJava() {
            var first = GlobalResources.LaunchInfoData.JavaRuntimes.Where(x => x.Is64Bit &&
            x.JavaSlugVersion == new GameCoreUtil(GlobalResources.LaunchInfoData.GameDirectoryPath)
            .GetGameCore(GlobalResources.LaunchInfoData.SelectGameCore).JavaVersion);

            if (first.Any()) {
                return first.First();
            } else {
                var second = GlobalResources.LaunchInfoData.JavaRuntimes.Where(x => x.JavaSlugVersion == new GameCoreUtil(GlobalResources.LaunchInfoData.GameDirectoryPath)
               .GetGameCore(GlobalResources.LaunchInfoData.SelectGameCore).JavaVersion);

                return second.Any() ? second.First() : GlobalResources.LaunchInfoData.JavaRuntimePath;
            }
        }

        private void ProcessOutput(object? sender, IProcessOutput e) {
            Trace.WriteLine(e.Raw);
        }
    }
}

//TODO: Fuck MacOS
#region Launch Script Create Method
//string java = (SystemUtils.IsWindows ? Path.Combine(new FileInfo(javaInfo!.JavaPath).Directory.FullName,"java.exe") : javaInfo!.JavaPath.ToFile().FullName);
//StringBuilder builder = new();
//if (SystemUtils.IsWindows) {
//    builder.AppendLine("@echo off");
//    builder.AppendLine($"set APPDATA={gameCore.Root.Root.FullName}");
//    builder.AppendLine($"set INST_NAME={gameCore.Id}");
//    builder.AppendLine($"set INST_ID={gameCore.Id}");
//    builder.AppendLine($"set INST_DIR={gameCore.GetGameCorePath()}");
//    builder.AppendLine($"set INST_MC_DIR={gameCore.Root.FullName}");
//    builder.AppendLine($"set INST_JAVA=\"{java}\"");
//    builder.AppendLine($"cd /D {gameCore.Root.FullName}");
//    builder.AppendLine($"\"{java}\" {string.Join(' '.ToString(), gameProcess.Arguemnts)}");
//    builder.AppendLine($"pause");
//} else if (SystemUtils.IsMacOS) {
//    builder.AppendLine($"export INST_NAME={gameCore.Id}");
//    builder.AppendLine($"export INST_ID={gameCore.Id}");
//    builder.AppendLine($"export INST_DIR=\"{gameCore.GetGameCorePath(launcher.EnableIndependencyCore)}\"");
//    builder.AppendLine($"export INST_MC_DIR=\"{gameCore.Root.FullName}\"");
//    builder.AppendLine($"export INST_JAVA=\"{java}\"");
//    builder.AppendLine($"cd \"{gameCore.Root!.FullName}\"");
//    builder.AppendLine($"\"{java}\" {string.Join(' '.ToString(), launcher.ArgumentsBuilder.Build())}");
//} else if (SystemUtils.IsLinux) {
//    builder.AppendLine($"export INST_JAVA={java}");
//    builder.AppendLine($"export INST_MC_DIR={gameCore.Root!.FullName}");
//    builder.AppendLine($"export INST_NAME={gameCore.Id}");
//    builder.AppendLine($"export INST_ID={gameCore.Id}");
//    builder.AppendLine($"export INST_DIR={gameCore.GetGameCorePath(launcher.EnableIndependencyCore)}");
//    builder.AppendLine($"cd {gameCore.Root!.FullName}");
//    builder.AppendLine($"{java} {string.Join(' '.ToString(), launcher.ArgumentsBuilder.Build())}");
//}

//File.WriteAllText(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "launchScript.bat"), builder.ToString());
#endregion
