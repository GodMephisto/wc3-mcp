---
type: vendor-doc
source_repo: "https://github.com/sgraf812/BananaMpq"
source_path: "BananaMpq.View/Views/TileSelectionEventArgs.cs"
ref: HEAD
---

using System;

namespace BananaMpq.View.Views
{
    public class TileSelectionEventArgs : EventArgs
    {
        public WowContinent Continent { get; private set; }
        public int X { get; private set; }
        public int Y { get; private set; }

        public TileSelectionEventArgs(WowContinent continent, int x, int y)
        {
            Y = y;
            X = x;
            Continent = continent;
        }
    }
}