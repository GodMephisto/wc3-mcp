---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/ProjectEditWorkspace.Menu.View.cs"
ref: HEAD
---

﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace vJassMainJBlueprint.V1.ProjectEditor
{
    partial class ProjectEditWorkspace
    {
        private void OnMenuViewSubMenuOpened(object sender, RoutedEventArgs e)
        {
            MenuViewToolSampleIcon.IsChecked = ToolSampleIcons.Visibility == Visibility.Visible;
        }

        private void OnMenuViewToolSampleIcon(object? sender, RoutedEventArgs? e)
        {
            if (ToolSampleIcons.Visibility == Visibility.Visible)
            {
                ToolSampleIcons.Visibility = Visibility.Collapsed;
            }
            else
            {
                ToolSampleIcons.Visibility = Visibility.Visible;
            }
        }

        private void OnMenuViewAllNodesClick(object? sender, RoutedEventArgs? e)
        {
            ViewportResetDelayed();
        }

        private DispatcherTimer? ViewportResetTimer = null;
        private bool ViewportResetQuiet = false;

        private void ViewportResetDelayed(bool quiet = false)
        {
            // 침묵 여부 설정
            ViewportResetQuiet = quiet;

            // 타이머가 없으면 생성
            if (ViewportResetTimer == null)
            {
                ViewportResetTimer = new() { Interval = TimeSpan.FromMilliseconds(100) };
                ViewportResetTimer.Tick += (sender, e) =>
                {
                    ViewportResetTimer.Stop();
                    Dispatcher.Invoke(() =>
                    {
                        ViewportReset(ViewportResetQuiet);
                    });
                };
            }
            // 타이머 재시작
            ViewportResetTimer.Stop();
            ViewportResetTimer.Start();
        }

        private void ViewportReset(bool quiet)
        {
            // 부모의 실제 너비와 높이
            double parentWidth = ZoomParent.ActualWidth;
            double parentHeight = ZoomParent.ActualHeight;

            // 자식 요소의 실제 너비와 높이
            double childWidth = ZoomChild.ActualWidth;
            double childHeight = ZoomChild.ActualHeight;

            // 가로와 세로 축소 비율 계산
            double scaleX = parentWidth / childWidth;
            double scaleY = parentHeight / childHeight;

            // 자식 요소의 확대/축소 비율을 가로, 세로 중 더 작은 값으로 설정
            double nextZoomFactor = Math.Min(scaleX, scaleY);

            // 확대 비율을 0.1에서 2 사이로 제한
            nextZoomFactor = Math.Max(0.1, Math.Min(2, nextZoomFactor));

            // 자식 요소의 ScaleTransform을 업데이트하여 확대/축소 적용
            var scaleTransform = (ScaleTransform)ZoomChild.LayoutTransform;
            scaleTransform.ScaleX = nextZoomFactor;
            scaleTransform.ScaleY = nextZoomFactor;

            // 부모 요소의 중앙에 자식 요소를 배치
            Canvas.SetLeft(ZoomChild, (parentWidth - childWidth * nextZoomFactor) / 2);
            Canvas.SetTop(ZoomChild, (parentHeight - childHeight * nextZoomFactor) / 2);

            // 확대/축소 비율 출력
            if (!quiet)
            {
                MessageText.Info($"확대/축소 {(int)(nextZoomFactor * 100)}%");
            }
        }
    }
}
