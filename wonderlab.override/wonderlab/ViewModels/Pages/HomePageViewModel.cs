﻿using MinecraftLaunch.Launch;
using MinecraftLaunch.Modules.Enum;
using MinecraftLaunch.Modules.Interface;
using MinecraftLaunch.Modules.Models.Auth;
using MinecraftLaunch.Modules.Models.Launch;
using MinecraftLaunch.Modules.Toolkits;
using Natsurainko.Toolkits.Network;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using wonderlab.Class.Models;
using wonderlab.Class.Utils;

namespace wonderlab.ViewModels.Pages
{
    public class HomePageViewModel : ReactiveObject
    {
        public HomePageViewModel() {
            this.PropertyChanged += OnPropertyChanged;
            HasGameCore = GameCores.Any() ? 0 : 1;
        }

        public bool Isopen { get; set; } = false;

        public Account CurrentAccount { get; set; } = Account.Default;

        [Reactive]
        public string SearchCondition { get; set; }

        [Reactive]
        public double SearchSuccess { get; set; } = 0;

        [Reactive]
        public double HasGameCore { get; set; } = 0;

        [Reactive]
        public double PanelHeight { get; set; } = 0;

        [Reactive]
        public GameCore SelectGameCore { get; set; }
        
        [Reactive]
        public ObservableCollection<GameCore> GameCores { get; set; } = new();

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(SearchCondition)) {
                SeachGameCore(SearchCondition);
            }

            if (e.PropertyName is nameof(SelectGameCore) && SelectGameCore != null) {
                App.LaunchInfoData.SelectGameCore = SelectGameCore.Id!;
            }
        }

        public async void SeachGameCore(string text) {
            if (!GameCores.Any()) {             
                return;
            }

            GameCores.Clear();
            GameCores = (await GameCoreUtils.SearchGameCoreAsync(App.LaunchInfoData.GameDirectoryPath, text))
                .Distinct().ToObservableCollection();

            if (!GameCores.Any()) {
                SearchSuccess = 1;
            }
            else SearchSuccess = 0;            
        }

        public async void GetGameCoresAction() {
            GameCores.Clear();
            var cores = await GameCoreUtils.GetLocalGameCores(App.LaunchInfoData.GameDirectoryPath);
            HasGameCore = cores.Any() ? 0 : 1;

            foreach (var i in cores) {
                await Task.Delay(20);    
                GameCores.Add(i);
            }
        }

        public async void SelectAccountAction() {
            var user = (await GameAccountUtils.GetUsersAsync());

            if (user.Count == 1) {
                CurrentAccount = user.First().ToAccount();
                LaunchTaskAction();
                return;
            }

            MainWindow.Instance.Auth.Show();            
        }

        public async void LaunchTaskAction() {
            $"开始尝试启动游戏 \"{SelectGameCore.Id}\"".ShowMessage();

            if (!Path.Combine(JsonUtils.DataPath, "authlib-injector.jar").IsFile()) {
                var result = await HttpWrapper.HttpDownloadAsync("https://download.mcbbs.net/mirrors/authlib-injector/artifact/45/authlib-injector-1.1.45.jar",
                    JsonUtils.DataPath, "authlib-injector.jar");

                Trace.WriteLine($"[信息] Http状态码为 {result.HttpStatusCode}");
            }

            var config = new LaunchConfig()
            {
                JvmConfig = new()
                {
                    AdvancedArguments = new List<string>() { GetJvmArguments() },
                    MaxMemory = App.LaunchInfoData.MaxMemory,
                    MinMemory = App.LaunchInfoData.MiniMemory,
                    JavaPath = GetCurrentJava().ToFile(),
                },
                
                Account = CurrentAccount,
                WorkingFolder = GameCoreUtils.GetGameCoreVersionPath(SelectGameCore).ToDirectory(),
            };

            JavaMinecraftLauncher launcher = new(config, App.LaunchInfoData.GameDirectoryPath, true);
            Stopwatch stopwatch = new();
            stopwatch.Start();
            using var gameProcess = await launcher.LaunchTaskAsync(App.LaunchInfoData.SelectGameCore, x => { 
                Trace.WriteLine($"[信息] {x.Item2}");
            });
            if (gameProcess.State is LaunchState.Succeess) {
                stopwatch.Stop();
                $"游戏 \"{App.LaunchInfoData.SelectGameCore}\" 已启动成功，总用时 {stopwatch.Elapsed}".ShowMessage("启动成功");

                gameProcess.Process.Exited += (sender, e) => {
                    Trace.WriteLine("[信息] 游戏退出！");
                };
                gameProcess.ProcessOutput += ProcessOutput;
            }
        }

        public string GetCurrentJava() {
            if (App.LaunchInfoData.IsAutoSelectJava) {
                var first = App.LaunchInfoData.JavaRuntimes.Where(x => x.Is64Bit && 
                x.JavaSlugVersion == new GameCoreToolkit(App.LaunchInfoData.GameDirectoryPath)
                .GetGameCore(App.LaunchInfoData.SelectGameCore).JavaVersion);                

                if (first.Any()) { 
                    return first.First().JavaPath.ToJavaw();   
                } else {
                    var second = App.LaunchInfoData.JavaRuntimes.Where(x => x.JavaSlugVersion == new GameCoreToolkit(App.LaunchInfoData.GameDirectoryPath)
                   .GetGameCore(App.LaunchInfoData.SelectGameCore).JavaVersion);

                    return second.Any() ? second.First().JavaPath.ToJavaw() : App.LaunchInfoData.JavaRuntimePath.JavaPath?.ToJavaw() ?? string.Empty;
                }
            }

            return App.LaunchInfoData.JavaRuntimePath.JavaPath.ToJavaw();
        }

        public string GetJvmArguments() {
            if (CurrentAccount.Type == AccountType.Yggdrasil) { 
                var account = CurrentAccount as YggdrasilAccount;
                return $"-javaagent:{Path.Combine(JsonUtils.DataPath, "authlib-injector.jar")}={account.YggdrasilServerUrl}";
            }

            return string.Empty;
        }

        private void ProcessOutput(object? sender, IProcessOutput e) {
            Trace.WriteLine(e.Raw);
        }
    }
}
