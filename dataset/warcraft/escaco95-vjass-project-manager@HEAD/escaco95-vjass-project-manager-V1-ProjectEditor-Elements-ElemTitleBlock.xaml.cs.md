---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/Elements/ElemTitleBlock.xaml.cs"
ref: HEAD
---

﻿using System.Windows;
using System.Windows.Controls;

namespace vJassMainJBlueprint.V1.ProjectEditor.Elements
{
    /// <summary>
    /// ElemTitleBlock.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ElemTitleBlock : UserControl
    {
        public ElemTitleBlock()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty ProjectNameProperty = DependencyProperty.Register("ProjectName", typeof(string), typeof(ElemTitleBlock), new PropertyMetadata("설계도 이름"));
        public static readonly DependencyProperty ProjectAuthorProperty = DependencyProperty.Register("ProjectAuthor", typeof(string), typeof(ElemTitleBlock), new PropertyMetadata("설계도 저자"));

        public string ProjectName { get { return (string)GetValue(ProjectNameProperty); } set { SetValue(ProjectNameProperty, value); } }
        public string ProjectAuthor { get { return (string)GetValue(ProjectAuthorProperty); } set { SetValue(ProjectAuthorProperty, value); } }
    }
}
