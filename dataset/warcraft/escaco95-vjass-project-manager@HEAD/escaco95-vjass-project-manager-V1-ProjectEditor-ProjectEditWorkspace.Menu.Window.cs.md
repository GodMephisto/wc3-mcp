---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/ProjectEditWorkspace.Menu.Window.cs"
ref: HEAD
---

﻿using System.Windows;

namespace vJassMainJBlueprint.V1.ProjectEditor
{
    partial class ProjectEditWorkspace
    {
        private void OnMenuWindowSubMenuOpened(object sender, RoutedEventArgs e)
        {
            if (Window.GetWindow(this) is not Window window) return;

            MenuWindowStayOnTop.IsChecked = window.Topmost;
        }

        private void OnMenuWindowStayOnTop(object sender, EventArgs e)
        {
            if (Window.GetWindow(this) is not Window window) return;

            window.Topmost = !window.Topmost;
            MenuWindowStayOnTop.IsChecked = window.Topmost;
        }
    }
}
