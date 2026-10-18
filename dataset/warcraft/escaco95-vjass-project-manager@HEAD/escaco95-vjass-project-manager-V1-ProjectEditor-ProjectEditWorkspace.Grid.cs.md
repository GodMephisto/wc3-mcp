---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/ProjectEditWorkspace.Grid.cs"
ref: HEAD
---

﻿using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shell;
using vJassMainJBlueprint.Utils;
using vJassMainJBlueprint.V1.Config;
using vJassMainJBlueprint.V1.ModelFacade;
using vJassMainJBlueprint.V1.ProjectEditor.Elements;

namespace vJassMainJBlueprint.V1.ProjectEditor
{
    partial class ProjectEditWorkspace
    {
        private void OnFooterZoomGridDragOver(object sender, DragEventArgs e)
        {
            e.Effects = e.Data.GetDataPresent(DataFormats.FileDrop) ? DragDropEffects.Copy : DragDropEffects.None;
            e.Handled = true;
        }

        private static readonly string[] separator = ["\r\n", "\n"];

        private void OnFooterZoomGridDrop(object sender, DragEventArgs e)
        {
            // Text 형식으로 파일 경로가 전달될 수 있음
            // e.g) VSCode에서 파일을 드래그 앤 드롭할 경우
            if (e.Data.GetDataPresent(DataFormats.Text))
            {
                try
                {
                    string filePathString = (string)e.Data.GetData(DataFormats.Text);

                    if (string.IsNullOrWhiteSpace(filePathString)) return;

                    string[] filePaths = filePathString.Split(separator, StringSplitOptions.RemoveEmptyEntries);

                    if (!filePaths.All(File.Exists)) return;

                    var dropWorldPosition = e.GetPosition(FooterZoomGrid);

                    var addedFileCount = ProjectSourceAdd([.. filePaths], (int)dropWorldPosition.X, (int)dropWorldPosition.Y);
                    MessageText.Info($"{addedFileCount}개의 파일을 추가했습니다.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"텍스트로 파일 경로를 처리하는 중 오류가 발생했습니다: {ex.Message}");
                }
            }
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                try
                {
                    string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                    if (files.Length == 0) return;

                    var dropWorldPosition = e.GetPosition(FooterZoomGrid);

                    var addedFileCount = ProjectSourceAdd([.. files], (int)dropWorldPosition.X, (int)dropWorldPosition.Y);
                    MessageText.Info($"{addedFileCount}개의 파일을 추가했습니다.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"파일을 추가하는 중 오류가 발생했습니다: {ex.Message}");
                }
            }
        }

        private readonly MouseInteractionState _mouseState = new();

        internal class MouseInteractionState
        {
            private readonly Dictionary<long, bool> SelectedNodeIdIndexes = [];
            private double SelectionRectPoint1X = 0;
            private double SelectionRectPoint1Y = 0;
            private double SelectionRectPoint2X = 0;
            private double SelectionRectPoint2Y = 0;
            internal bool IsSelecting = false;
            internal bool IsResizing = false;
            internal bool IsDragging = false;
            internal bool IsVieportDragging = false;
            internal ElemNode? EditingNode = null;
            internal readonly Dictionary<ElemNode, Point> LastMouseOffsets = [];
            internal Point LastMousePosition = new(0, 0);
            internal ResizeGripDirection LastResizeDirection = ResizeGripDirection.None;

            internal bool IsBusy => IsSelecting || IsResizing || IsDragging || IsVieportDragging;

            internal bool SelectedNothing => SelectedNodeCount == 0;

            internal bool SelectedSingle => SelectedNodeCount == 1;

            internal bool SelectedMultiple => SelectedNodeCount > 1;

            private int SelectedNodeCount => SelectedNodeIdIndexes.Count;

            internal IEnumerable<long> SelectedNodeIds => [.. SelectedNodeIdIndexes.Keys];

            internal void SelectionAdd(long nodeHandleId)
            {
                SelectedNodeIdIndexes.Add(nodeHandleId, true);
            }

            internal bool IsSelected(long nodeHandleId) => SelectedNodeIdIndexes.ContainsKey(nodeHandleId);

            internal void SelectionClear()
            {
                SelectedNodeIdIndexes.Clear();
            }

            internal void SelectionStartPoint(Point point)
            {
                SelectionRectPoint1X = point.X;
                SelectionRectPoint1Y = point.Y;
                SelectionRectPoint2X = point.X;
                SelectionRectPoint2Y = point.Y;
            }

            internal void SelectionEndPoint(Point point)
            {
                SelectionRectPoint2X = point.X;
                SelectionRectPoint2Y = point.Y;
            }

            internal Rect SelectionBounds
            {
                get
                {
                    double left = Math.Min(SelectionRectPoint1X, SelectionRectPoint2X);
                    double top = Math.Min(SelectionRectPoint1Y, SelectionRectPoint2Y);
                    double width = Math.Abs(SelectionRectPoint1X - SelectionRectPoint2X);
                    double height = Math.Abs(SelectionRectPoint1Y - SelectionRectPoint2Y);
                    return new(left, top, width, height);
                }
            }
        }
        private ElemNode? GetTopMostNode(Point mousePosition)
        {
            HitTestResult hitResult = VisualTreeHelper.HitTest(NodeContainer, mousePosition);

            return GetParentElemNode(hitResult?.VisualHit);
        }
        private static ElemNode? GetParentElemNode(DependencyObject? child)
        {
            // 현재 child가 null이거나 ElemNode일 때까지 반복
            while (child != null && child is not ElemNode)
            {
                // 부모 요소 탐색
                child = VisualTreeHelper.GetParent(child);
            }

            // ElemNode일 경우에만 반환
            return child as ElemNode;
        }

        private void OnGridMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (_mouseState.IsBusy) return;

            switch (e.ChangedButton)
            {
                case MouseButton.Left:
                    {
                        Point mousePosition = e.GetPosition(ZoomChild);
                        // 마우스 위치와 겹치는 TopMost 노드 선택
                        ElemNode? topMostNode = GetTopMostNode(mousePosition);
                        if (projectEditFacade.SelectNodeSourceFilePath(topMostNode?.SourceNodeID) is string sourceFilePath)
                        {
                            // 상대 경로인 경우 문서 기준 절대 경로로 변환
                            string absoluteSourceFilePath = PathHelper.ConvertToAbsolutePath(projectEditFacade.OriginFilePath, sourceFilePath);
                            Debug.WriteLine($"Open: {absoluteSourceFilePath}");
                            ProcessHelper.Open(Window.GetWindow(this), absoluteSourceFilePath);
                        }
                    }
                    break;
                case MouseButton.Middle:
                    {
                        Point mousePosition = e.GetPosition(ZoomChild);
                        // 마우스 위치와 겹치는 TopMost 노드 선택
                        ElemNode? topMostNode = GetTopMostNode(mousePosition);
                        if (topMostNode is null)
                        {
                            _mouseState.IsVieportDragging = true;
                            _mouseState.LastMousePosition = e.GetPosition(ZoomParent);
                            WorkspaceGrid.CaptureMouse(); // 마우스 캡처

                            // 찍은 좌표 노출
                            MessageText.Info($"X:{(int)mousePosition.X} Y:{(int)mousePosition.Y}");
                            break;
                        }
                        // 선택되지 않은 노드를 클릭 시, 선택된 노드 초기화
                        if (!_mouseState.IsSelected(topMostNode.SourceNodeID))
                        {
                            SelectionClear();
                            SelectionAdd(topMostNode.SourceNodeID);
                        }
                        // 드래그 또는 크리 조정 시작
                        var resizeDirection = UIResizeHelper.GetResizeDirection(topMostNode, e.GetPosition(topMostNode));
                        if (resizeDirection == ResizeGripDirection.None)
                        {
                            _mouseState.IsDragging = true;
                            _mouseState.EditingNode = topMostNode;
                            _mouseState.LastMouseOffsets.Clear();
                            foreach (long selectedNodeHandleId in _mouseState.SelectedNodeIds)
                            {
                                var selectedNode = nodeElements[selectedNodeHandleId];
                                _mouseState.LastMouseOffsets[selectedNode] = e.GetPosition(selectedNode);
                            }
                            topMostNode.CaptureMouse(); // 마우스 캡처
                                                        // 스냅 커서 표시
                            SnapCursor.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            _mouseState.IsResizing = true;
                            _mouseState.EditingNode = topMostNode;
                            _mouseState.LastMousePosition = mousePosition;
                            _mouseState.LastResizeDirection = resizeDirection;
                            _mouseState.LastMouseOffsets.Clear();
                            foreach (long selectedNodeHandleId in _mouseState.SelectedNodeIds)
                            {
                                var selectedNode = nodeElements[selectedNodeHandleId];
                                _mouseState.LastMouseOffsets[selectedNode] = new Point(selectedNode.ActualWidth, selectedNode.ActualHeight);
                            }
                            topMostNode.CaptureMouse(); // 마우스 캡처
                        }
                    }
                    break;
                case MouseButton.Right:
                    {
                        _mouseState.IsSelecting = true;
                        // 마우스 좌클릭 시, 선택된 노드 초기화
                        WorkspaceGrid.CaptureMouse();
                        SelectionClear();
                        Point mousePosition = e.GetPosition(ZoomChild);
                        _mouseState.SelectionStartPoint(mousePosition);
                        Rect selectionBounds = _mouseState.SelectionBounds;
                        double zoomFactor = ((ScaleTransform)ZoomChild.LayoutTransform).ScaleX;
                        Canvas.SetLeft(SelectionGrid, Canvas.GetLeft(ZoomChild) + selectionBounds.Left * zoomFactor);
                        Canvas.SetTop(SelectionGrid, Canvas.GetTop(ZoomChild) + selectionBounds.Top * zoomFactor);
                        SelectionGrid.Width = selectionBounds.Width * zoomFactor;
                        SelectionGrid.Height = selectionBounds.Height * zoomFactor;
                        SelectionGrid.Visibility = Visibility.Visible;
                    }
                    break;
            }
        }

        private void OnGridMouseUp(object sender, MouseButtonEventArgs e)
        {
            switch (e.ChangedButton)
            {
                case MouseButton.Middle:
                    {
                        if (_mouseState.IsDragging)
                        {
                            projectEditFacade.UpdateNodePosition(
                                _mouseState.SelectedNodeIds
                                    .Select(nodeHandleId => nodeElements[nodeHandleId])
                                    .Select(node =>
                                    new ProjectEditFacade.NodePositionUpdateRequest
                                    {
                                        NodeHandleId = node!.SourceNodeID,
                                        X = (int)Canvas.GetLeft(node),
                                        Y = (int)Canvas.GetTop(node),
                                    }).ToList());
                            projectEditFacade.UpdateOriginSaveRequired(true);
                            _mouseState.EditingNode?.ReleaseMouseCapture(); // 마우스 캡처 해제
                                                                            // 스냅 커서 숨김
                            SnapCursor.Visibility = Visibility.Collapsed;
                            // 만약 하나의 객체만 선택되어 있었을 경우, 선택 해제
                            if (_mouseState.SelectedSingle)
                            {
                                SelectionClear();
                            }
                        }
                        else if (_mouseState.IsResizing)
                        {
                            projectEditFacade.UpdateNodeSize(
                                _mouseState.SelectedNodeIds
                                    .Select(nodeHandleId => nodeElements[nodeHandleId])
                                    .Select(node =>
                                    new ProjectEditFacade.NodeSizeUpdateRequest
                                    {
                                        NodeHandleId = node!.SourceNodeID,
                                        Width = (int)node!.ActualWidth,
                                        Height = (int)node!.ActualHeight,
                                    }).ToList());
                            projectEditFacade.UpdateOriginSaveRequired(true);
                            _mouseState.EditingNode?.ReleaseMouseCapture(); // 마우스 캡처 해제
                                                                            // 만약 하나의 객체만 선택되어 있었을 경우, 선택 해제
                            if (_mouseState.SelectedSingle)
                            {
                                SelectionClear();
                            }
                        }
                        else if (_mouseState.IsVieportDragging)
                        {
                            WorkspaceGrid.ReleaseMouseCapture(); // 마우스 캡처 해제
                        }
                        _mouseState.IsDragging = false;
                        _mouseState.IsResizing = false;
                        _mouseState.IsVieportDragging = false;
                    }
                    break;
                case MouseButton.Right:
                    {
                        // 마우스 선택 영역에 맞게 사각형 생성 (마우스 드래그 방향에 따라 Point1과 Point2를 조정)
                        Rect selectionRect = _mouseState.SelectionBounds;
                        // 선택 영역에 포함된 노드 선택
                        SelectionAdd(NodeContainer.Children.OfType<ElemNode>()
                            .Where(node => selectionRect.IntersectsWith(new Rect(Canvas.GetLeft(node), Canvas.GetTop(node), node.ActualWidth, node.ActualHeight)))
                            .Where(node => !_mouseState.IsSelected(node.SourceNodeID))
                            .Select(node => node.SourceNodeID)
                            .ToList());
                        // 마우스 좌클릭 해제 시, 마우스 캡쳐 해제
                        WorkspaceGrid.ReleaseMouseCapture();
                        SelectionGrid.Visibility = Visibility.Collapsed;
                        _mouseState.IsSelecting = false;
                    }
                    break;
            }
        }

        private void OnGridMouseMove(object sender, MouseEventArgs e)
        {
            if (_mouseState.IsVieportDragging)
            {
                Point currentPosition = e.GetPosition(ZoomParent);

                Mouse.OverrideCursor = Cursors.ScrollAll;

                Canvas.SetLeft(ZoomChild, Canvas.GetLeft(ZoomChild) - _mouseState.LastMousePosition.X + currentPosition.X);
                Canvas.SetTop(ZoomChild, Canvas.GetTop(ZoomChild) - _mouseState.LastMousePosition.Y + currentPosition.Y);

                _mouseState.LastMousePosition = currentPosition;
            }
            else if (_mouseState.IsDragging)
            {
                const int gridSize = JassProjectSpecification.SnapThreshold;
                foreach (var (draggingNode, nodeDragOffset) in _mouseState.LastMouseOffsets)
                {
                    Point currentPosition = e.GetPosition(ZoomChild);
                    // snap to grid (10x10 based on parent element offset)
                    currentPosition.X = currentPosition.X - nodeDragOffset.X;
                    currentPosition.Y = currentPosition.Y - nodeDragOffset.Y;
                    // cap to parent element bounds
                    currentPosition.X = Math.Max(0, Math.Min(NodeContainer.ActualWidth - draggingNode.ActualWidth, currentPosition.X));
                    currentPosition.Y = Math.Max(0, Math.Min(NodeContainer.ActualHeight - draggingNode.ActualHeight, currentPosition.Y));
                    // snap to grid (10x10 based on parent element offset)
                    currentPosition.X = (int)(currentPosition.X / gridSize) * gridSize;
                    currentPosition.Y = (int)(currentPosition.Y / gridSize) * gridSize;
                    // 요소 이동
                    Canvas.SetLeft(draggingNode, currentPosition.X);
                    Canvas.SetTop(draggingNode, currentPosition.Y);
                    // 스냅 커서 이동
                    if (draggingNode == _mouseState.EditingNode)
                    {
                        SnapCursor.SetPosition(currentPosition.X, currentPosition.Y);
                    }
                }
            }
            else if (_mouseState.IsResizing)
            {
                Point currentMousePosition = e.GetPosition(ZoomChild);
                int deltaX = (int)(currentMousePosition.X - _mouseState.LastMousePosition.X);
                int deltaY = (int)(currentMousePosition.Y - _mouseState.LastMousePosition.Y);
                foreach (var item in _mouseState.LastMouseOffsets)
                {
                    var lastElementSize = item.Value;
                    // 요소 크기 조정
                    switch (_mouseState.LastResizeDirection)
                    {
                        case ResizeGripDirection.Right:
                            {
                                int width = (int)(lastElementSize.X + deltaX);
                                width = Math.Max(30, Math.Min((int)(FooterZoomGrid.Width - Canvas.GetLeft(item.Key)), width));
                                item.Key.Width = JassProjectSpecification.Snap(width);
                                break;
                            }
                        case ResizeGripDirection.Bottom:
                            {
                                int height = (int)(lastElementSize.Y + deltaY);
                                height = Math.Max(30, Math.Min((int)(FooterZoomGrid.Height - Canvas.GetTop(item.Key)), height));
                                item.Key.Height = JassProjectSpecification.Snap(height);
                                break;
                            }
                        case ResizeGripDirection.BottomRight:
                            {
                                int width = (int)(lastElementSize.X + deltaX);
                                width = Math.Max(30, Math.Min((int)(FooterZoomGrid.Width - Canvas.GetLeft(item.Key)), width));
                                int height = (int)(lastElementSize.Y + deltaY);
                                height = Math.Max(30, Math.Min((int)(FooterZoomGrid.Height - Canvas.GetTop(item.Key)), height));
                                item.Key.Width = JassProjectSpecification.Snap(width);
                                item.Key.Height = JassProjectSpecification.Snap(height);
                            }
                            break;
                    }
                }
            }
            else if (_mouseState.IsSelecting)
            {
                Point mousePosition = e.GetPosition(ZoomChild);
                _mouseState.SelectionEndPoint(mousePosition);
                Rect selectionBounds = _mouseState.SelectionBounds;
                double zoomFactor = ((ScaleTransform)ZoomChild.LayoutTransform).ScaleX;
                Canvas.SetLeft(SelectionGrid, Canvas.GetLeft(ZoomChild) + selectionBounds.Left * zoomFactor);
                Canvas.SetTop(SelectionGrid, Canvas.GetTop(ZoomChild) + selectionBounds.Top * zoomFactor);
                SelectionGrid.Width = selectionBounds.Width * zoomFactor;
                SelectionGrid.Height = selectionBounds.Height * zoomFactor;
            }
            else if (_mouseState.SelectedNothing)
            {
                Point mousePosition = e.GetPosition(ZoomChild);
                // 마우스 위치와 겹치는 TopMost 노드 선택
                ElemNode? topMostNode = NodeContainer.Children.OfType<ElemNode>()
                    .Where(node => new Rect(Canvas.GetLeft(node), Canvas.GetTop(node), node.ActualWidth, node.ActualHeight).Contains(mousePosition))
                    .OrderBy(node => Panel.GetZIndex(node))
                    .FirstOrDefault();
                if (topMostNode is null)
                {
                    Mouse.OverrideCursor = null;
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.Hand;
                }
            }
            else
            {
                Point mousePosition = e.GetPosition(ZoomChild);
                // 마우스 위치와 겹치는 TopMost 노드 선택
                ElemNode? topMostNode = NodeContainer.Children.OfType<ElemNode>()
                    .Where(node => new Rect(Canvas.GetLeft(node), Canvas.GetTop(node), node.ActualWidth, node.ActualHeight).Contains(mousePosition))
                    .OrderBy(node => Panel.GetZIndex(node))
                    .FirstOrDefault();
                if (topMostNode is null)
                {
                    Mouse.OverrideCursor = null;
                }
                else
                {
                    var resizeDirection = UIResizeHelper.GetResizeDirection(topMostNode, e.GetPosition(topMostNode));
                    Mouse.OverrideCursor = resizeDirection switch
                    {
                        ResizeGripDirection.Right => Cursors.SizeWE,
                        ResizeGripDirection.Bottom => Cursors.SizeNS,
                        ResizeGripDirection.BottomRight => Cursors.SizeNWSE,
                        _ => Cursors.Hand,
                    };
                }
            }
        }

        private void OnGridMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // 줌 적용 전, ZoomParent 좌표계에서 ZoomChild 내부에서의 좌표 (마우스 커서 위치 기준)
            Point cursorPositionInZoomParent = e.GetPosition(ZoomParent);

            // 줌 인/아웃에 따른 줌 팩터 적용
            double previousZoomFactor = ((ScaleTransform)ZoomChild.LayoutTransform).ScaleX;
            double nextZoomFactor = e.Delta > 0 ? previousZoomFactor * 1.1 : previousZoomFactor / 1.1;
            nextZoomFactor = Math.Max(0.1, Math.Min(2, nextZoomFactor));

            // 줌 적용
            var scaleTransform = (ScaleTransform)ZoomChild.LayoutTransform;
            scaleTransform.ScaleX = nextZoomFactor;
            scaleTransform.ScaleY = nextZoomFactor;

            // 현재 자식 정사각형에서 마우스의 상대적 위치 계산
            double relativeMousePosX = cursorPositionInZoomParent.X - Canvas.GetLeft(ZoomChild);
            double relativeMousePosY = cursorPositionInZoomParent.Y - Canvas.GetTop(ZoomChild);

            // 확대/축소 비율 계산 (X축과 Y축 각각)
            double scaleX = (ZoomChild.ActualWidth * nextZoomFactor) / (ZoomChild.ActualWidth * previousZoomFactor);
            double scaleY = (ZoomChild.ActualHeight * nextZoomFactor) / (ZoomChild.ActualHeight * previousZoomFactor);

            // 새로운 좌상단 좌표를 계산
            double newChildPosX = cursorPositionInZoomParent.X - (relativeMousePosX * scaleX);
            double newChildPosY = cursorPositionInZoomParent.Y - (relativeMousePosY * scaleY);

            Canvas.SetLeft(ZoomChild, newChildPosX);
            Canvas.SetTop(ZoomChild, newChildPosY);

            MessageText.Info($"확대/축소 {(int)(nextZoomFactor * 100)}%");
        }

        private void SelectionAdd(long nodeHandleId) => SelectionAdd([nodeHandleId]);

        private void SelectionAdd(List<long> nodeIds)
        {
            nodeIds.Where(nodeHandleId => !_mouseState.IsSelected(nodeHandleId))
                .ToList().ForEach(nodeHandleId =>
                {
                    // 논리 선택 갱신
                    _mouseState.SelectionAdd(nodeHandleId);
                    // 비주얼 선택
                    nodeElements[nodeHandleId].MarkAsSelected();
                });
        }

        private void SelectionClear()
        {
            // 비주얼 선택 해제
            _mouseState.SelectedNodeIds.Select(nodeHandleId => nodeElements[nodeHandleId]).ToList().ForEach(node => node.MarkAsUnselected());
            // 논리 선택 초기화
            _mouseState.SelectionClear();
        }
    }
}
