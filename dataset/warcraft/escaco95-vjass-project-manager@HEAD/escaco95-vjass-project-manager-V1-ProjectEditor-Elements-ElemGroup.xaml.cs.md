---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/Elements/ElemGroup.xaml.cs"
ref: HEAD
---

﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;
using vJassMainJBlueprint.Utils;
using vJassMainJBlueprint.V1.Model;
using vJassMainJBlueprint.V1.ProjectEditor.Overlays;

namespace vJassMainJBlueprint.V1.ProjectEditor.Elements
{
    /// <summary>
    /// ElemGroup.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ElemGroup : UserControl
    {
        private readonly DispatcherTimer _collapseTimer = new() { Interval = TimeSpan.FromMilliseconds(500) };
        private JassProject.GroupTextAlignment _align = JassProject.GroupTextAlignment.TopLeft;
        private Color _background = Color.FromArgb(255, 0x71, 0x60, 0xE8);
        private Color _foreground = Colors.White;

        public ElemGroup()
        {
            InitializeComponent();

            _collapseTimer.Tick += AfterMouseLeave;
        }

        public ElemGroup(JassProject.Group group) : this()
        {
            Canvas.SetLeft(this, group.X);
            Canvas.SetTop(this, group.Y);
            Width = group.Width;
            Height = group.Height;
            TextBlockLabel.Text = group.Text;
            TextBlockLabel.FontSize = group.FontSize;
            _align = group.Align;
            _foreground = group.Foreground;
            _background = group.Background;
            Backdrop.Fill = new SolidColorBrush(_background);
            BackdropBorder.BorderBrush = new SolidColorBrush(_background);
            MoveHandle.Background = new SolidColorBrush(_background);
            ResizeHandle.Background = new SolidColorBrush(_background);
            TextBlockLabel.Foreground = new SolidColorBrush(_foreground);

            RefreshAlign();
        }

        private void OnMouseEnter(object sender, MouseEventArgs e)
        {
            MoveHandle.Visibility = Visibility.Visible;
            ResizeHandle.Visibility = Visibility.Visible;
            _collapseTimer.Stop();
        }

        private void OnMouseLeave(object sender, MouseEventArgs e)
        {
            _collapseTimer.Stop();
            _collapseTimer.Start();
        }

        private void AfterMouseLeave(object? sender, EventArgs e)
        {
            MoveHandle.Visibility = Visibility.Collapsed;
            ResizeHandle.Visibility = Visibility.Collapsed;
        }

        private void OnMouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (_isResizing || _isMoving) return;

            if (e.LeftButton != MouseButtonState.Pressed) return;

            if (new Rect(0, 0, TextBlockLabel.ActualWidth, TextBlockLabel.ActualHeight).Contains(e.GetPosition(this)))
            {
                ActionRenameByUser();
            }
        }

        private bool _isResizing = false;
        private bool _isMoving = false;
        private Point _lastClickedOffset;
        private Point _lastSize;

        private void OnMouseLeftButtonDown(object sender, MouseEventArgs e)
        {
            // is busy then do nothing
            if (_isResizing || _isMoving) return;

            if (new Rect(0, 0, MoveHandle.ActualWidth, MoveHandle.ActualHeight).Contains(e.GetPosition(MoveHandle)))
            {
                _isMoving = true;
                _lastSize = new Point(Canvas.GetLeft(this), Canvas.GetTop(this));
                _lastClickedOffset = e.GetPosition((IInputElement)Parent);
                MoveHandle.CaptureMouse();
            }
            else if (new Rect(0, 0, ResizeHandle.ActualWidth, ResizeHandle.ActualHeight).Contains(e.GetPosition(ResizeHandle)))
            {
                _isResizing = true;
                _lastSize = new Point(Width, Height);
                _lastClickedOffset = e.GetPosition(this);
                ResizeHandle.CaptureMouse();
            }
        }

        private void OnMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if (_isMoving)
            {
                MoveHandle.ReleaseMouseCapture();
                _isMoving = false;
                Messenger.Send(new ProjectEditWorkspace.SaveRequireMessage());
            }
            else if (_isResizing)
            {
                ResizeHandle.ReleaseMouseCapture();
                _isResizing = false;
                Messenger.Send(new ProjectEditWorkspace.SaveRequireMessage());
            }
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (_isMoving)
            {
                var pos = e.GetPosition((IInputElement)Parent);
                var deltaX = pos.X - _lastClickedOffset.X;
                var deltaY = pos.Y - _lastClickedOffset.Y;
                var snappedX = (int)deltaX / 10 * 10;
                var snappedY = (int)deltaY / 10 * 10;
                var cappedX = Math.Max(0, _lastSize.X + snappedX);
                var cappedY = Math.Max(0, _lastSize.Y + snappedY);
                Canvas.SetLeft(this, cappedX);
                Canvas.SetTop(this, cappedY);
            }
            else if (_isResizing)
            {
                var pos = e.GetPosition(this);
                var deltaX = pos.X - _lastClickedOffset.X;
                var deltaY = pos.Y - _lastClickedOffset.Y;
                var snappedX = (int)deltaX / 10 * 10;
                var snappedY = (int)deltaY / 10 * 10;
                var cappedWidth = Math.Max(30, _lastSize.X + snappedX);
                var cappedHeight = Math.Max(30, _lastSize.Y + snappedY);
                this.Width = cappedWidth;
                this.Height = cappedHeight;
            }
        }

        private void OnContextMenuRename(object sender, RoutedEventArgs e) => ActionRenameByUser();

        private void OnContextMenuFontSize(object sender, RoutedEventArgs e)
        {
            if (InputBox.Show(this, "폰트 크기 변경", "문구 크기 변경 (최소 10, 최대 100)", $"{TextBlockLabel.FontSize}") is not string fontSizeString) return;

            if (!int.TryParse(fontSizeString, out int fontSize)) return;

            if (fontSize == (int)TextBlockLabel.FontSize) return;

            if (fontSize < 10 || fontSize > 100)
            {
                Messenger.Send(new OverlayMessageBox.ShowActionMessage("폰트 크기가 잘못되었습니다.", OverlayMessageBox.ShowActionMessage.LevelType.Error));
                return;
            }

            TextBlockLabel.FontSize = fontSize;
            Messenger.Send(new OverlayMessageBox.ShowActionMessage("문구 크기를 변경했습니다."));
            Messenger.Send(new ProjectEditWorkspace.SaveRequireMessage());
        }

        private void OnContextMenuForeground(object sender, RoutedEventArgs e)
        {
            if (InputBox.Show(this, "문구 색상 변경", "문구 색상 변경 (#AARRGGBB, 16진수)", ColorHelper.ToHex(_foreground)) is not string colorString) return;

            if (!ColorHelper.TryParse(colorString, out Color color) || color == _foreground) return;

            _foreground = color;
            TextBlockLabel.Foreground = new SolidColorBrush(_foreground);
            Messenger.Send(new OverlayMessageBox.ShowActionMessage("전경색을 변경했습니다."));
            Messenger.Send(new ProjectEditWorkspace.SaveRequireMessage());
        }

        private void OnContextMenuBackground(object sender, RoutedEventArgs e)
        {
            if (InputBox.Show(this, "배경 색상 변경", "배경 색상 변경 (#AARRGGBB, 16진수)", ColorHelper.ToHex(_background)) is not string colorString) return;

            if (!ColorHelper.TryParse(colorString, out Color color) || color == _background) return;

            _background = color;
            Backdrop.Fill = new SolidColorBrush(_background);
            BackdropBorder.BorderBrush = new SolidColorBrush(_background);
            MoveHandle.Background = new SolidColorBrush(_background);
            ResizeHandle.Background = new SolidColorBrush(_background);
            Messenger.Send(new OverlayMessageBox.ShowActionMessage("배경색을 변경했습니다."));
            Messenger.Send(new ProjectEditWorkspace.SaveRequireMessage());
        }

        private void OnContextMenuTextAlignOpen(object sender, RoutedEventArgs e)
        {
            if (sender is not MenuItem menuItem) return;

            foreach (MenuItem subItem in menuItem.Items)
            {
                subItem.IsChecked = Enum.TryParse((string)subItem.Tag, out JassProject.GroupTextAlignment align) && align == _align;
            }
        }

        private void OnContextMenuTextAlign(object sender, RoutedEventArgs e)
            => ActionToggleAlignByUser((Enum.TryParse((string)((MenuItem)sender).Tag, out JassProject.GroupTextAlignment align)) ? align : null);

        private void OnContextMenuDelete(object sender, RoutedEventArgs e) => ActionDeleteByUser();

        private void ActionRenameByUser()
        {
            if (InputBox.Show(this, "라벨 수정", "텍스트 입력", TextBlockLabel.Text) is not string changedText) return;

            if (changedText.Equals(TextBlockLabel.Text)) return;

            TextBlockLabel.Text = changedText;
            Messenger.Send(new OverlayMessageBox.ShowActionMessage("라벨 텍스트를 변경했습니다."));
            Messenger.Send(new ProjectEditWorkspace.SaveRequireMessage());
        }

        private void ActionToggleAlignByUser(JassProject.GroupTextAlignment? align)
        {
            if (!align.HasValue) return;
            if (_align == align.Value) return;

            _align = align.Value;
            RefreshAlign();
            Messenger.Send(new ProjectEditWorkspace.SaveRequireMessage());
        }

        private void ActionDeleteByUser()
        {
            if (Parent is Canvas parent)
            {
                parent.Children.Remove(this);
            }
            Messenger.Send(new ProjectEditWorkspace.SaveRequireMessage());
        }

        public JassProject.Group ToModel()
        {
            return new JassProject.Group(
                (int)Canvas.GetLeft(this),
                (int)Canvas.GetTop(this),
                (int)Width,
                (int)Height,
                TextBlockLabel.Text,
                (int)TextBlockLabel.FontSize,
                _align,
                _foreground,
                _background);
        }

        private void RefreshAlign()
        {
            switch (_align)
            {
                case JassProject.GroupTextAlignment.TopLeft:
                    TextBlockLabel.TextAlignment = TextAlignment.Left;
                    TextBlockLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    TextBlockLabel.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case JassProject.GroupTextAlignment.TopCenter:
                    TextBlockLabel.TextAlignment = TextAlignment.Center;
                    TextBlockLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    TextBlockLabel.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case JassProject.GroupTextAlignment.TopRight:
                    TextBlockLabel.TextAlignment = TextAlignment.Right;
                    TextBlockLabel.HorizontalAlignment = HorizontalAlignment.Right;
                    TextBlockLabel.VerticalAlignment = VerticalAlignment.Top;
                    break;
                case JassProject.GroupTextAlignment.MiddleLeft:
                    TextBlockLabel.TextAlignment = TextAlignment.Left;
                    TextBlockLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    TextBlockLabel.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case JassProject.GroupTextAlignment.MiddleCenter:
                    TextBlockLabel.TextAlignment = TextAlignment.Center;
                    TextBlockLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    TextBlockLabel.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case JassProject.GroupTextAlignment.MiddleRight:
                    TextBlockLabel.TextAlignment = TextAlignment.Right;
                    TextBlockLabel.HorizontalAlignment = HorizontalAlignment.Right;
                    TextBlockLabel.VerticalAlignment = VerticalAlignment.Center;
                    break;
                case JassProject.GroupTextAlignment.BottomLeft:
                    TextBlockLabel.TextAlignment = TextAlignment.Left;
                    TextBlockLabel.HorizontalAlignment = HorizontalAlignment.Left;
                    TextBlockLabel.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case JassProject.GroupTextAlignment.BottomCenter:
                    TextBlockLabel.TextAlignment = TextAlignment.Center;
                    TextBlockLabel.HorizontalAlignment = HorizontalAlignment.Center;
                    TextBlockLabel.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
                case JassProject.GroupTextAlignment.BottomRight:
                    TextBlockLabel.TextAlignment = TextAlignment.Right;
                    TextBlockLabel.HorizontalAlignment = HorizontalAlignment.Right;
                    TextBlockLabel.VerticalAlignment = VerticalAlignment.Bottom;
                    break;
            }
        }
    }
}
