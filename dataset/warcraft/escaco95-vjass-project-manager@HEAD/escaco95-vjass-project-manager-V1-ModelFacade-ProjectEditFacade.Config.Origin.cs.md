---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ModelFacade/ProjectEditFacade.Config.Origin.cs"
ref: HEAD
---

﻿namespace vJassMainJBlueprint.V1.ModelFacade
{
    partial class ProjectEditFacade
    {
        public bool OriginSaveRequired => _originConfig.OriginSaveRequired;

        public OriginType Origin => _originConfig.Origin;

        public string OriginFilePath => _originConfig.OriginFilePath;

        public void UpdateOrigin(string filePath)
        {
            _originConfig.ResetToFile(filePath);
            UpdateRequired?.Invoke(this, [new ProjectOriginUpdateEventArgs(_originConfig), new ProjectOriginSaveRequireEventArgs(_originConfig)]);
        }

        public void UpdateOriginSaveRequired(bool saveRequired)
        {
            if (_originConfig.OriginSaveRequired == saveRequired) return;

            _originConfig.OriginSaveRequired = saveRequired;
            UpdateRequired?.Invoke(this, [new ProjectOriginSaveRequireEventArgs(_originConfig)]);
        }
    }
}
