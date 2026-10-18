---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/Elements/ElemMeasureVertical.xaml.cs"
ref: HEAD
---

﻿using System.Windows;
using System.Windows.Controls;

namespace vJassMainJBlueprint.V1.ProjectEditor.Elements
{
    /// <summary>
    /// ElemMeasureVertical.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ElemMeasureVertical : UserControl
    {
        public ElemMeasureVertical()
        {
            InitializeComponent();
        }

        // DependencyProperty for Text
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ElemMeasureVertical), new PropertyMetadata("설계도 높이"));

        // CLR wrapper for Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
