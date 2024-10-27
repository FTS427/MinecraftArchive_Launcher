using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Threading;
using ReactiveUI;
using System;
using System.Linq;
using System.Reactive.Concurrency;
using System.Threading.Tasks;
using MinecraftArchive.Class.AppData;
using MinecraftArchive.Class.Enum;
using MinecraftArchive.Class.Utils;
using MinecraftArchive.control;
using MinecraftArchive.ViewModels.Windows;
using MinecraftArchive.Views.Pages;
using static MinecraftArchive.control.Controls.Bar.MessageTipsBar;

namespace MinecraftArchive.Views.Windows {
    public partial class MainWindow : Window {
        public bool CanParallax;

        public static MainWindowViewModel? ViewModel { get; set; }

        public MainWindow() {
            InitializeComponent();
            RxApp.MainThreadScheduler.Schedule(LoadAllDataAsync);
        }

        private void MainWindowLoaded(object sender, RoutedEventArgs e) {
            try {
                PointerMoved += OnPointerMoved;
                PropertyChanged += OnPropertyChanged;
                Drop.PointerPressed += OnPointerPressed;
                TopInfoBar.PointerPressed += OnPointerPressed;
                TopInfoBar1.PointerPressed += OnPointerPressed;
                TopInfoBar2.PointerPressed += OnPointerPressed;

                close.Click += (_, _) => Close();
                Mini.Click += (_, _) => WindowState = WindowState.Minimized;

                NotificationCenterButton.Click += (_, _) => NotificationCenter.Open();
                CanParallax = GlobalResources.LauncherData.ParallaxType is not "��";
            }
            catch (Exception ex) {
                $"{ex.Message}".ShowMessage();
            }
        }

        private void OnPointerMoved(object? sender, PointerEventArgs e) {
            if (CanParallax) {
                var control = BackgroundImage;
                ParallaxUtil.RunFlatParallax(control, e.GetPosition(control));
            } else {
                BackgroundImage.RenderTransform = new TranslateTransform(0, 0);
            }
        }

        private void OnPropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e) {
            if (e.Property == HeightProperty) {
                if (ViewModel.CurrentPage is HomePage) {
                    if (HomePage.ViewModel.Isopen) {
                        HomePage.ViewModel.PanelHeight = Height - 180;
                    }
                }
            }
        }

        private void OnPointerPressed(object? sender, PointerPressedEventArgs e) {
            BeginMoveDrag(e);
        }

        public async void DropAction(object? sender, DragEventArgs e) {
            try {
                "��⵽���룡".ShowLog();
                var result = e.Data.GetFiles();

                if (!result.IsNull() && !result.Any()) {
                    return;
                }

                if (result!.Count() > 1) {
                    "һ��ֻ������һ���ļ�".ShowMessage("��ʾ");
                    return;
                }

                "��ʼ�����ļ����ͣ��˹��̻�����������룬�����ĵȴ�".ShowMessage("��ʾ");
                var file = result!.FirstOrDefault()!.ToFile();
                if (!(file.Name.EndsWith(".zip") || file.Name.EndsWith(".mrpack"))) {
                    $"WonderLab δ��ȷ���ļ� {file.Name} �ĸ�ʽӦ��ִ�е���ز���".ShowMessage("����");
                    return;
                }
                var type = ModpacksUtils.ModpacksTypeAnalysis(file.FullName);
                if (type is ModpacksType.Unknown) {
                    "δ֪���ϰ�����".ShowMessage();
                    return;
                }
                await ModpacksUtils.ModpacksInstallAsync(file.FullName, type);
            }
            catch (Exception) {
                "Fuck".ShowLog(LogLevel.Error);
            }
        }

        public void ShowInfoBar(string title, string message, HideOfRunAction action) {
            tipBarView.Add(title, message, action);
        }

        public void ShowInfoBar(string title, string message) {
            Dispatcher.UIThread.Post(() => {
                tipBarView.Add(title, message);
            }, DispatcherPriority.Background);
        }

        public async void Navigation(UserControl control) {
            try {
                Page.Opacity = 0;
                await Task.Delay(200);
                Page.Content = control;
                await Task.Delay(150);
                Page.Opacity = 1;
            }
            catch (Exception) { }
        }

        public async void CloseTopBar() {
            TopBar1.Margin = new(0, -150, 0, 0);
            await Task.Delay(50);
            TopBar2.Margin = new(0, -100, 0, 0);
            await Task.Delay(50);
            TopBar3.Margin = new(0, -100, 0, 0);
        }

        public async void OpenTopBar() {
            TopBar3.Margin = new(0);
            await Task.Delay(50);
            TopBar2.Margin = new(0);
            await Task.Delay(50);
            TopBar1.Margin = new(0);
        }

        public async void LoadAllDataAsync() {
            ThemeUtils.Init();
            ThemeUtils.SetAccentColor(GlobalResources.LauncherData.AccentColor);

            JsonUtils.CreateLauncherInfoJson();
            JsonUtils.CreateLaunchInfoJson();
            await Task.Run(async () => {
                CacheResources.Accounts = (await AccountUtils.GetAsync(true)
                     .ToListAsync())
                     .ToObservableCollection();

                CacheResources.GetWebModpackInfoData();
            });

            FontFamily = ExtendUtils.UseSystemFont();
            AddHandler(DragDrop.DropEvent, DropAction);
        }
    }
}