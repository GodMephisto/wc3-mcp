---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Views/TreeChunkHierarchyView.xaml.cs"
ref: HEAD
---

﻿using System.Collections.Generic;
using System.Windows.Controls;
using BananaMpq.View.Models;

namespace BananaMpq.View.Views
{
    public partial class TreeChunkHierarchyView : UserControl, IChunkHierarchyView
    {
        public TreeChunkHierarchyView()
        {
            InitializeComponent();
        }

        #region Implementation of IChunkHierarchyView

        public IEnumerable<ChunkHierarchyModel> Hierarchy
        {
            set { _treeView.ItemsSource = value; }
        }

        #endregion
    }
}
