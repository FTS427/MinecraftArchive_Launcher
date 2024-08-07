﻿using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Models;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.control.Animation;
using MinecraftArchive.control.Controls.Bar;
using MinecraftArchive.Views.Pages;
using MinecraftArchive.Views.Windows;

namespace MinecraftArchive.ViewModels.Pages {
    public class ActionCenterPageViewModel : ViewModelBase {
        public ActionCenterPageViewModel() {
            PropertyChanged += OnPropertyChanged;
            //Task.Factory.StartNew(GetMojangNewsAction);
            //Task.Factory.StartNew(GetHitokotoAction);
            //Task.Factory.StartNew(GetLatestGameCoreAction);
        }

        public void OnPropertyChanged(object? sender, PropertyChangedEventArgs e) {

        }

        [Reactive]
        public string NewTitle { get; set; } = "Loading...";

        [Reactive]
        public string NewTag { get; set; } = "Loading...";

        [Reactive]
        public string HitokotoTitle { get; set; }

        [Reactive]
        public string LatestGameCore { get; set; }

        [Reactive]
        public string HitokotoCreator { get; set; }

        [Reactive]
        public Bitmap NewImage { get; set; }

        public async void GetMojangNewsAction() {
            try {
                New result = null;
                if (CacheResources.MojangNews.Count <= 0) {
                    List<New> news = new(await HttpUtils.GetMojangNewsAsync());
                    result = news.Count > 0 ? news.First() : new();
                } else {
                    result = CacheResources.MojangNews.FirstOrDefault()!;
                }

                NewTitle = result.Title;
                NewTag = result.Tag;
                if (result.NewsPageImage != null) {
                    string url = $"https://launchercontent.mojang.com/{result.NewsPageImage.Url}";
                    NewImage = NewImage.IsNull() ? await HttpUtils.GetWebBitmapAsync(url) : NewImage;
                }
            }
            catch (HttpRequestException ex) {
                $"哎哟，获取失败力，请检查您的网络是否正常，详细信息：{ex.Message}".ShowMessage();
            }
        }

        public async void GetHitokotoAction() {
            var result = await HttpUtils.GetHitokotoTextAsync();

            if (result != null) {
                HitokotoCreator ??= $"-- {result.Creator?.Trim()}";
                HitokotoTitle ??= result.Text ?? "焯";
            }
        }

        public async void GetLatestGameCoreAction() {
            try {
                var result = await HttpUtils.GetLatestGameCoreAsync();
                LatestGameCore = result;
            }
            catch (Exception ex) {
                $"焯，{ex}".ShowLog();
            }
        }

        public void OpenInstallDialogAction() {
            new DownCenterPage().Navigation();
        }

        public void OpenSelectConfigPageAction() {
            new SelectConfigPage().Navigation();
        }

        public void OpenUserPageAction() {
            new AccountPage().Navigation();
        }
    }
}