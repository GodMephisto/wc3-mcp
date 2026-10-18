---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/Overlays/OverlayMessageBox.xaml.cs"
ref: HEAD
---

﻿using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using vJassMainJBlueprint.Utils;

namespace vJassMainJBlueprint.V1.ProjectEditor.Overlays
{
    /// <summary>
    /// OverlayMessageBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class OverlayMessageBox : UserControl
    {
        private static readonly TimeSpan DefaultFadeDuration = TimeSpan.FromSeconds(0.5);
        private static readonly TimeSpan DefaultVisibleDuration = TimeSpan.FromSeconds(0.5);
        private readonly DispatcherTimer _collapseTimer = new();

        public OverlayMessageBox()
        {
            InitializeComponent();

            _collapseTimer.Tick += (sender, e) =>
            {
                _collapseTimer.Stop();
                Visibility = System.Windows.Visibility.Collapsed;
            };
        }

        private void UserControl_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            BlockRoot.Opacity = 0;
            Messenger.Subscribe<ShowActionMessage>((message) => { Dispatcher.Invoke(() => ShowMessage(message)); });
        }

        public void Info(string message)
        {
            ShowMessage(new ShowActionMessage(message, ShowActionMessage.LevelType.Info));
        }

        public void Warn(string message)
        {
            ShowMessage(new ShowActionMessage(message, ShowActionMessage.LevelType.Warn));
        }

        public void Error(string message)
        {
            ShowMessage(new ShowActionMessage(message, ShowActionMessage.LevelType.Error));
        }

        private void ShowMessage(ShowActionMessage message)
        {
            MessageTextBlock.Text = message.Message;
            BlockBackdrop.Background = GetMessageLevelBrush(message.Level);

            BlockRoot.BeginAnimation(OpacityProperty, null);

            Visibility = System.Windows.Visibility.Visible;
            BlockRoot.Opacity = 1;

            DoubleAnimation fadeOutAnimation = new(0, message.FadeDuration) { BeginTime = message.VisibleDuration };

            _collapseTimer.Stop();
            _collapseTimer.Interval = message.FadeDuration + message.VisibleDuration;
            _collapseTimer.Start();

            BlockRoot.BeginAnimation(OpacityProperty, fadeOutAnimation);
        }

        private static SolidColorBrush GetMessageLevelBrush(ShowActionMessage.LevelType level)
        {
            return level switch
            {
                ShowActionMessage.LevelType.Info => Brushes.Blue,
                ShowActionMessage.LevelType.Warn => Brushes.Orange,
                ShowActionMessage.LevelType.Error => Brushes.Red,
                _ => Brushes.Black,
            };
        }

        public readonly struct ShowActionMessage
        {
            public enum LevelType
            {
                Info,
                Warn,
                Error,
            }

            public readonly string Message;
            public readonly LevelType Level;
            public readonly TimeSpan FadeDuration;
            public readonly TimeSpan VisibleDuration;

            public ShowActionMessage(string message)
            {
                Message = message;
                Level = LevelType.Info;
                FadeDuration = DefaultFadeDuration;
                VisibleDuration = DefaultVisibleDuration;
            }

            public ShowActionMessage(string message, LevelType level)
            {
                Message = message;
                Level = level;
                FadeDuration = DefaultFadeDuration;
                VisibleDuration = DefaultVisibleDuration;
            }
        }
    }
}
