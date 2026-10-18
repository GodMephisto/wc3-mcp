---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/Elements/ElemMeasureHorizontal.xaml.cs"
ref: HEAD
---

﻿using System.Windows;
using System.Windows.Controls;

namespace vJassMainJBlueprint.V1.ProjectEditor.Elements
{
    /// <summary>
    /// ElemMeasureHorizontal.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ElemMeasureHorizontal : UserControl
    {
        public ElemMeasureHorizontal()
        {
            InitializeComponent();
        }

        // DependencyProperty for Text
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ElemMeasureHorizontal), new PropertyMetadata("설계도 너비"));

        // CLR wrapper for Text
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }
    }
}
