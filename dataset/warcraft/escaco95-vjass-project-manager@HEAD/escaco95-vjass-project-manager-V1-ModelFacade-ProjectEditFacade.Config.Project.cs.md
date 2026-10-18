---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ModelFacade/ProjectEditFacade.Config.Project.cs"
ref: HEAD
---

﻿namespace vJassMainJBlueprint.V1.ModelFacade
{
    partial class ProjectEditFacade
    {
        public string GetProjectName() => _projectConfig.Name;

        public string GetProjectAuthor() => _projectConfig.Author;

        public int GetProjectWidth() => _projectConfig.Width;

        public int GetProjectHeight() => _projectConfig.Height;

        public bool UpdateProjectName(string name)
        {
            if (_projectConfig.Name == name) return false;

            _projectConfig.Name = name;

            UpdateRequired?.Invoke(this, [new ProjectNameChangedEventArgs(_projectConfig)]);

            return true;
        }

        public bool UpdateProjectAuthor(string author)
        {
            if (_projectConfig.Author == author) return false;

            _projectConfig.Author = author;

            UpdateRequired?.Invoke(this, [new ProjectAuthorChangedEventArgs(_projectConfig)]);

            return true;
        }

        public bool UpdateProjectSize(int width, int height)
        {
            if (_projectConfig.Width == width && _projectConfig.Height == height) return false;

            _projectConfig.Width = width;
            _projectConfig.Height = height;

            UpdateRequired?.Invoke(this, [new ProjectResizeEventArgs(_projectConfig)]);

            return true;
        }
    }
}
