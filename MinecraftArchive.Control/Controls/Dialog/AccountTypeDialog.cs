﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Input;
using Avalonia.Interactivity;
using MinecraftArchive.control.Animation;
using MinecraftArchive.control.Interface;

namespace MinecraftArchive.control.Controls.Dialog {
    public class AccountTypeDialog : TemplatedControl,IDialog {
        private Button OfflineButton = null!;

        private Button YggdrasilButton = null!;

        private Button MicrosoftButton = null!;

        private Border Content = null!;

        private Border BackgroundBorder = null!;

        public ICommand OfflineCommand { get => GetValue(OfflineCommandProperty); set => SetValue(OfflineCommandProperty, value); }

        public ICommand YggdrasilCommand { get => GetValue(YggdrasilCommandProperty); set => SetValue(YggdrasilCommandProperty, value); }

        public ICommand MicrosoftCommand { get => GetValue(MicrosoftCommandProperty); set => SetValue(MicrosoftCommandProperty, value); }

        public static readonly StyledProperty<ICommand> OfflineCommandProperty =
            AvaloniaProperty.Register<AccountTypeDialog, ICommand>(nameof(OfflineCommand));

        public static readonly StyledProperty<ICommand> YggdrasilCommandProperty =
            AvaloniaProperty.Register<AccountTypeDialog, ICommand>(nameof(YggdrasilCommand));

        public static readonly StyledProperty<ICommand> MicrosoftCommandProperty =
            AvaloniaProperty.Register<AccountTypeDialog, ICommand>(nameof(MicrosoftCommand));

        public void HideDialog() {
            BackgroundBorder.IsHitTestVisible = false;
            Content.IsHitTestVisible = false;

            OpacityChangeAnimation animation = new(true);
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(Content);
        }

        public void ShowDialog() {
            BackgroundBorder.IsHitTestVisible = true;
            Content.IsHitTestVisible = true;

            OpacityChangeAnimation animation = new(false) {
                RunValue = 0
            };
            animation.RunAnimation(BackgroundBorder);
            animation.RunAnimation(Content);
        }

        protected override void OnApplyTemplate(TemplateAppliedEventArgs e) {
            base.OnApplyTemplate(e);

            BackgroundBorder = e.NameScope.Find<Border>("BackgroundBorder")!;
            Content = e.NameScope.Find<Border>("DialogContent")!;
            OfflineButton = e.NameScope.Find<Button>("OfflineButton")!;
            YggdrasilButton = e.NameScope.Find<Button>("YggdrasilButton")!;
            MicrosoftButton = e.NameScope.Find<Button>("MicrosoftButton")!;

            OfflineButton.Click += OnClick;
            MicrosoftButton.Click += OnClick;
            YggdrasilButton.Click += OnClick;
            e.NameScope.Find<Button>("CloseButton")!.Click += OnClick;
            HideDialog();
        }

        private void OnClick(object? sender, RoutedEventArgs e) {
            HideDialog();
        }

    }
}
