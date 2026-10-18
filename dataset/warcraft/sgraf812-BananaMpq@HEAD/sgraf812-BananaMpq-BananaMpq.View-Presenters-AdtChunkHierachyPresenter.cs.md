---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Presenters/AdtChunkHierachyPresenter.cs"
ref: HEAD
---

﻿using System;
using BananaMpq.View.Infrastructure;
using BananaMpq.View.Models;
using BananaMpq.View.Views;

namespace BananaMpq.View.Presenters
{
    public class AdtChunkHierachyPresenter
    {
        private readonly SceneService _service;
        private readonly IChunkHierarchyView _view;

        public AdtChunkHierachyPresenter(IChunkHierarchyView view)
        {
            _view = view;
            _service = SceneService.Instance;
            _service.TileLoaded += DisplayAdtHierarchy;
        }

        private void DisplayAdtHierarchy(object s, EventArgs e)
        {
            _view.Hierarchy = new ChunkHierarchyModel(null, _service.Scene.Adt).Children;
        }
    }
}