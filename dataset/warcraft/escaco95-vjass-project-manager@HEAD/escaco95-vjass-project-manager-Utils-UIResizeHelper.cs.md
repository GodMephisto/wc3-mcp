---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "Utils/UIResizeHelper.cs"
ref: HEAD
---

﻿using System.Windows;
using System.Windows.Shell;

namespace vJassMainJBlueprint.Utils
{
    public static class UIResizeHelper
    {
        private const int ResizeGripSize = 5;

        public static ResizeGripDirection GetResizeDirection(UIElement uIElement, Point mousePos)
        {
            if (mousePos.X < ResizeGripSize && mousePos.Y < ResizeGripSize)
            {
                return ResizeGripDirection.TopLeft;
            }
            else if (mousePos.X > uIElement.RenderSize.Width - ResizeGripSize && mousePos.Y < ResizeGripSize)
            {
                return ResizeGripDirection.TopRight;
            }
            else if (mousePos.X < ResizeGripSize && mousePos.Y > uIElement.RenderSize.Height - ResizeGripSize)
            {
                return ResizeGripDirection.BottomLeft;
            }
            else if (mousePos.X > uIElement.RenderSize.Width - ResizeGripSize && mousePos.Y > uIElement.RenderSize.Height - ResizeGripSize)
            {
                return ResizeGripDirection.BottomRight;
            }
            else if (mousePos.X < ResizeGripSize)
            {
                return ResizeGripDirection.Left;
            }
            else if (mousePos.X > uIElement.RenderSize.Width - ResizeGripSize)
            {
                return ResizeGripDirection.Right;
            }
            else if (mousePos.Y < ResizeGripSize)
            {
                return ResizeGripDirection.Top;
            }
            else if (mousePos.Y > uIElement.RenderSize.Height - ResizeGripSize)
            {
                return ResizeGripDirection.Bottom;
            }
            else
            {
                return ResizeGripDirection.None;
            }
        }
    }
}
