---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ModelFacade/ProjectEditFacade.Project.cs"
ref: HEAD
---

﻿using vJassMainJBlueprint.V1.Model;

namespace vJassMainJBlueprint.V1.ModelFacade
{
    partial class ProjectEditFacade
    {
        public JassProject GetProject(JassProject.Group[] groups)
        {
            JassProject.Node[] nodes = _nodeCollectionConfigs.NodeConfigs.Values.Select(nodeConfig => nodeConfig.Cast()).ToArray();
            JassProject project = _projectConfig.Cast(nodes,groups);

            return project;
        }

        public void MakeNewProject()
        {
            ProjectCleanUp();

            _originConfig.Reset();
            _projectConfig.Reset();
            _nodeCollectionConfigs.Reset();

            ProjectSetUp();
        }

        public void MakeNewProject(string fileName, JassProject jassProject)
        {
            ProjectCleanUp();

            _originConfig.ResetToFile(fileName);
            _projectConfig.Consume(jassProject);
            _nodeCollectionConfigs.Consume(jassProject);

            ProjectSetUp();
        }

        private void ProjectCleanUp()
        {
            List<IUpdateRequiredEventArgs> projectCleanupEventArgs = [];

            foreach (var item in _nodeCollectionConfigs.NodeConfigs)
            {
                projectCleanupEventArgs.Add(new NodeRemoveEventArgs(item.Key));
            }

            UpdateRequired?.Invoke(this, [.. projectCleanupEventArgs]);
        }

        private void ProjectSetUp()
        {
            List<IUpdateRequiredEventArgs> projectSetupEventArgs = [];

            projectSetupEventArgs.Add(new ProjectOriginUpdateEventArgs(_originConfig));
            projectSetupEventArgs.Add(new ProjectOriginSaveRequireEventArgs(_originConfig));
            projectSetupEventArgs.Add(new ProjectNameChangedEventArgs(_projectConfig));
            projectSetupEventArgs.Add(new ProjectAuthorChangedEventArgs(_projectConfig));
            projectSetupEventArgs.Add(new ProjectResizeEventArgs(_projectConfig));
            foreach (var (_, nodeConfig) in _nodeCollectionConfigs.NodeConfigs)
            {
                projectSetupEventArgs.Add(new NodeAddEventArgs(nodeConfig));
            }

            UpdateRequired?.Invoke(this, [.. projectSetupEventArgs]);
        }
    }
}
