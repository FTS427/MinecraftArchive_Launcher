﻿using Avalonia.Platform.Storage;
using MinecraftLaunch.Launch;
using MinecraftLaunch.Utilities;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive.Linq;
using System.Threading;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.control;

namespace MinecraftArchive.ViewModels.Pages {
    public class LaunchConfigPageViewModel : ViewModelBase {
        public LaunchConfigPageViewModel() {
            try {
                if (GlobalResources.LaunchInfoData.JavaRuntimes.Any()) {
                    ThreadPool.QueueUserWorkItem(x => {
                        Javas = GlobalResources.LaunchInfoData.JavaRuntimes.ToObservableCollection();
                        CurrentJava = GlobalResources.LaunchInfoData.JavaRuntimes
                           .FirstOrDefault(x=> x.JavaPath == GlobalResources.LaunchInfoData.JavaRuntimePath.JavaPath);
                    });
                }

                if (GlobalResources.LaunchInfoData.GameDirectorys.Any()) {
                    CurrentGameDirectory = GlobalResources.LaunchInfoData.GameDirectoryPath;
                    GameDirectorys = GlobalResources.LaunchInfoData.GameDirectorys.ToObservableCollection();
                }

                ThreadPool.QueueUserWorkItem(x => {
                    MaxMemory = GlobalResources.LaunchInfoData.MaxMemory;
                    PropertyChanged += OnPropertyChanged;
                });

                WindowHeight = GlobalResources.LaunchInfoData.WindowHeight;
                WindowWidth = GlobalResources.LaunchInfoData.WindowWidth;
                IsAutoSelectJava = GlobalResources.LaunchInfoData.IsAutoSelectJava;
                IsAutoGetMemory = GlobalResources.LaunchInfoData.IsAutoGetMemory;
                JvmArgument = GlobalResources.LaunchInfoData.JvmArgument;
            }
            catch (Exception ex) {
                $"{ex.Message}".ShowMessage();
            }

            GetMemoryAction();
        }

        private void OnPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e) {
            if (e.PropertyName is nameof(CurrentJava)) {
                GlobalResources.LaunchInfoData.JavaRuntimePath = CurrentJava;
            }

            if (e.PropertyName is nameof(CurrentGameDirectory)) {
                GlobalResources.LaunchInfoData.GameDirectoryPath = CurrentGameDirectory;
            }

            if (e.PropertyName is nameof(MaxMemory)) {
                GlobalResources.LaunchInfoData.MaxMemory = MaxMemory;
            }

            if (e.PropertyName is nameof(IsAutoSelectJava)) {
                GlobalResources.LaunchInfoData.IsAutoSelectJava = IsAutoSelectJava;
            }

            if (e.PropertyName is nameof(WindowHeight)) {
                GlobalResources.LaunchInfoData.WindowHeight = WindowHeight;
            }

            if (e.PropertyName is nameof(WindowWidth)) {
                GlobalResources.LaunchInfoData.WindowWidth = WindowWidth;
            }

            if (e.PropertyName is nameof(IsAutoGetMemory)) {
                GlobalResources.LaunchInfoData.IsAutoGetMemory = IsAutoGetMemory;
            }

            if (e.PropertyName is nameof(JvmArgument)) {
                GlobalResources.LaunchInfoData.JvmArgument = JvmArgument;
            }
        }

        [Reactive]
        public bool IsLoadJavaFinish { get; set; } = true;

        [Reactive]
        public bool IsAutoSelectJava { get; set; } = false;

        [Reactive]
        public bool IsAutoGetMemory { get; set; } = false;

        [Reactive]
        public bool IsLoadJavaNow { get; set; } = false;

        [Reactive]
        public int MaxMemory { get; set; } = 6;

        [Reactive]
        public int TotalMemory { get; set; } = 0;

        [Reactive]
        public int FreeMemory { get; set; } = 0;

        [Reactive]
        public int WindowWidth { get; set; } = 854;

        [Reactive]
        public int WindowHeight { get; set; } = 480;

        [Reactive]
        public int CurrentAction { get; set; } = 1;

        [Reactive]
        public JavaInfo CurrentJava { get; set; }

        [Reactive]
        public string CurrentGameDirectory { get; set; }

        [Reactive]
        public string JvmArgument { get; set; }

        [Reactive]
        public ObservableCollection<JavaInfo> Javas { get; set; } = new();

        [Reactive]
        public ObservableCollection<string> GameDirectorys { get; set; } = new();

        public ObservableCollection<string> Actions => new()
        {
            "最小化",
            "无任何行为",
            "关闭启动器",
        };

        public async void LoadJavaAction() {
            IsLoadJavaFinish = false;
            IsLoadJavaNow = true;

            //在搜索前清除已有的搜索结果
            Javas.Clear();

            await Task.Run(async () => {
                try {
                    var result = await Task.Run(() => JavaUtil.GetJavas());
                    if (!result.IsNull() && result.Any()) {
                        Javas.Load(result);
                        GlobalResources.LaunchInfoData.JavaRuntimes = result.ToList();
                    }

                    CurrentJava = Javas.Last();
                }
                catch (Exception ex) {
                    $"{ex}".ShowInfoDialog("程序遭遇了异常");
                }

                IsLoadJavaFinish = true;
                IsLoadJavaNow = false;
            });
        }

        public void FileSearchAsync(DirectoryInfo directory, string pattern) {
            try {
                foreach (FileInfo fi in directory.GetFiles(pattern).Where(x => !x.IsReadOnly
                && directory.Attributes != FileAttributes.ReadOnly && directory.Attributes != FileAttributes.System
                && directory.Attributes != FileAttributes.Hidden)) {
                    AddRelativeDocument(fi.FullName);
                }

                foreach (DirectoryInfo di in directory.GetDirectories().Where(x => directory.Attributes != FileAttributes.ReadOnly)) {
                    if (!di.FullName.StartsWith(Environment.GetFolderPath(Environment.SpecialFolder.Windows))) {
                        FileSearchAsync(di, pattern);
                    }
                }
            }
            catch (Exception) {

            }
        }

        public void AddRelativeDocument(string path) {
            FileInfo fileInfo = new FileInfo(path);
            DirectoryInfo? directory = fileInfo.Directory;
            if (directory != null) {
                var javaInfo = JavaUtil.GetJavaInfo(Path.Combine(path1: directory.FullName, "java.exe"));
                Javas.Add(javaInfo);
                GlobalResources.LaunchInfoData.JavaRuntimes.Add(JavaUtil.GetJavaInfo(path));
                CurrentJava = javaInfo;
                Trace.WriteLine($"[Info] 这是第 {Javas.Count} 找到的 Java 运行时，完整路径为 {path}");
            }
        }

        public async void AddJavaAction() {
            var file = await DialogUtils.OpenFilePickerAsync(new List<FilePickerFileType>() {
                new("Java文件") { Patterns = new List<string>() { SystemUtils.IsWindows ? "javaw.exe" : "java" } }
            }, "请选择您的 Java 文件");

            if (!file.IsNull()) {
                //由于需启动新进程，可能耗时会卡主线程，因此使用异步
                var java = await Task.Run(() => JavaUtil.GetJavaInfo(file.FullName));
                if (!java.IsNull()) {
                    Javas.Add(java);
                    GlobalResources.LaunchInfoData.JavaRuntimes.Add(java);
                    CurrentJava = java;
                }
            }
        }

        public async void DirectoryDialogOpenAction() {
            var folder = await DialogUtils.OpenFolderPickerAsync("请选择一个游戏目录");
            if (!folder.IsNull()) {
                GameDirectorys.Add(folder.FullName);
                GlobalResources.LaunchInfoData.GameDirectorys.Add(folder.FullName);
                CurrentGameDirectory = folder.FullName;
            }
        }

        public void RemoveDirectoryAction() {
            GlobalResources.LaunchInfoData.GameDirectorys.Remove(CurrentGameDirectory);
            GameDirectorys.Remove(CurrentGameDirectory);
            CurrentGameDirectory = GameDirectorys.Any() ? GameDirectorys.First() : string.Empty;
        }

        public void RemoveJavaRuntimeAction() {
            var current = CurrentJava;
            Javas.Remove(current);
            GlobalResources.LaunchInfoData.JavaRuntimes.Remove(current);
            CurrentJava = Javas.Any() ? Javas.First() : null!;
        }

        public void CloseAutoSelectJavaAction() {
            IsAutoSelectJava = false;
        }

        public void OpenAutoSelectJavaAction() {
            IsAutoSelectJava = true;
        }

        public void CloseAutoGetMemoryAction() {
            IsAutoGetMemory = false;
        }

        public void OpenAutoGetMemoryAction() {
            IsAutoGetMemory = true;
        }

        public async void GetMemoryAction() {
            await Task.Run(async () => {
                while (true) {
                    var result = SystemUtils.GetMemoryInfo();
                    FreeMemory = result.Free.ToInt32();
                    TotalMemory = result.Total.ToInt32();
                    await Task.Delay(2000);
                }
            });
        }
    }
}
