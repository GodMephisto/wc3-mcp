---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/Elements/ElemNode.xaml.cs"
ref: HEAD
---

﻿using System.Windows.Controls;
using System.Windows.Media.Imaging;
using static vJassMainJBlueprint.V1.ModelFacade.ProjectEditFacade;

namespace vJassMainJBlueprint.V1.ProjectEditor.Elements
{
    /// <summary>
    /// ElemNode.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ElemNode : UserControl
    {

        public long SourceNodeID { get; private set; }

        public ElemNode()
        {
            InitializeComponent();
        }

        public ElemNode(NodeAddEventArgs e) : this()
        {
            SourceNodeID = e.NodeHandleId;

            UpdateBounds(e.X, e.Y, e.Width, e.Height);
            UpdateSourceFilePath(e.SourceFilePath);
            UpdateImage(e.Image);
        }

        public void UpdateNode(NodeBoundUpdateEventArgs e) => UpdateBounds(e.X, e.Y, e.Width, e.Height);

        public void UpdateNode(NodeSourceFilePathUpdateEventArgs e) => UpdateSourceFilePath(e.SourceFilePath);

        public void UpdateNode(NodeImageUpdateEventArgs e) => UpdateImage(e.Image);

        private void UpdateBounds(int x, int y, int width, int height)
        {
            Canvas.SetLeft(this, x);
            Canvas.SetTop(this, y);
            Width = width;
            Height = height;
        }

        private void UpdateSourceFilePath(string sourceFilePath)
        {
            if (sourceFilePath.StartsWith("..\\"))
            {
                ToolTip = sourceFilePath;
                BorderAbsolutePathWarn.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                ToolTip = $"(절대 경로)\n{sourceFilePath}\n\n타인에게 설계도 공유 시 올바르게 동작하지 않을 수 있습니다.\n - 이 파일은 절대 경로로 지정되어 있습니다";
                BorderAbsolutePathWarn.Visibility = System.Windows.Visibility.Visible;
            }
        }

        private void UpdateImage(BitmapImage? image)
        {
            ImageBox.Source = image;
        }

        public void MarkAsSelected()
        {
            SelectionBorder.Visibility = System.Windows.Visibility.Visible;
        }

        public void MarkAsUnselected()
        {
            SelectionBorder.Visibility = System.Windows.Visibility.Collapsed;
        }
    }
}
