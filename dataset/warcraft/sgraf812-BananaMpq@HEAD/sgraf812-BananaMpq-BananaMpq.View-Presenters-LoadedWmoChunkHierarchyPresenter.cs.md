---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Presenters/LoadedWmoChunkHierarchyPresenter.cs"
ref: HEAD
---

﻿using System;
using System.Linq;
using BananaMpq.View.Infrastructure;
using BananaMpq.View.Models;
using BananaMpq.View.Views;

namespace BananaMpq.View.Presenters
{
    public class LoadedWmoChunkHierarchyPresenter
    {
        private readonly SceneService _service;
        private readonly IChunkHierarchyView _view;

        public LoadedWmoChunkHierarchyPresenter(IChunkHierarchyView view)
        {
            _view = view;
            _service = SceneService.Instance;
            _service.TileLoaded += DisplayWmoHierarchy;
        }

        private void DisplayWmoHierarchy(object sender, EventArgs eventArgs)
        {
            _view.Hierarchy = _service.Scene.Adt.WmoReferences.Select(r => 
                new ChunkHierarchyModel(r.String, _service.Files.GetWmo(r.String)));
        }
    }
}