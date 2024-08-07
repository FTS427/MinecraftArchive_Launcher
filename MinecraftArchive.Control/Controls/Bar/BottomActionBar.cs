﻿using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MinecraftArchive.control.Controls.Bar
{
    /// <summary>
    /// 底部操作栏
    /// </summary>
    public class BottomActionBar : TemplatedControl
    {
        public event EventHandler<EventArgs>? LaunchButtonClick;
        public event EventHandler<EventArgs>? GameChangeClick;
        public ICommand? LaunchButtonCommand { get => GetValue(LaunchButtonCommandProperty); set => SetValue(LaunchButtonCommandProperty, value); }
        public string? GameCoreId { get => GetValue(GameCoreIdProperty); set => SetValue(GameCoreIdProperty, value); }
        public string? SelectState { get => GetValue(SelectStateProperty); set => SetValue(SelectStateProperty, value); }

        //Property
        public static readonly StyledProperty<ICommand> LaunchButtonCommandProperty =
            AvaloniaProperty.Register<BottomActionBar, ICommand>(nameof(LaunchButtonCommand));

        public static readonly StyledProperty<string> GameCoreIdProperty =
            AvaloniaProperty.Register<BottomActionBar, string>(nameof(GameCoreId));

        public static readonly StyledProperty<string> SelectStateProperty =
            AvaloniaProperty.Register<BottomActionBar, string>(nameof(SelectState), "未选择任何游戏核心");

        protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change) {       
            base.OnPropertyChanged(change);

            if (change.Property == GameCoreIdProperty) {
                if (string.IsNullOrEmpty(GameCoreId)) {
                    SelectState = "未选择任何游戏核心";
                } else SelectState= $"当前选择的游戏核心";
            }
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {       
            base.OnApplyTemplate(e);
            
            var res = e.NameScope.Find<Button>("LaunchButton");
            var res1 = e.NameScope.Find<Button>("GameChangeButton");
            res.Click += OnLaunchClick;
            res1.Click += OnGameChangeClick;
        }

        private void OnGameChangeClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {
            GameChangeClick?.Invoke(sender, e);
        }

        private void OnLaunchClick(object? sender, Avalonia.Interactivity.RoutedEventArgs e) {       
            LaunchButtonClick?.Invoke(sender, e);
            LaunchButtonCommand?.Execute(null);
        }
    }
}
