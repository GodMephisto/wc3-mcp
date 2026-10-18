---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/Features/TopMenu.File.Recent.cs"
ref: HEAD
---

﻿using System.IO;
using System.Windows;
using vJassMainJBlueprint.Utils;
using vJassMainJBlueprint.V1.ModelHelper;
using vJassMainJBlueprint.V1.ProjectEditor.Elements;

namespace vJassMainJBlueprint.V1.ProjectEditor
{
    partial class ProjectEditWorkspace
    {
        [FeatureManager.FeatureSubscriber]
        private class FeatureTopMenuFileRecent
        {
            // 기능 초기화
            private FeatureTopMenuFileRecent()
            {
                FeatureManager.SubscribeToFeature<ProjectEditWorkspace>(InitializeFeature);
            }

            // 기능 적용
            private static void InitializeFeature(ProjectEditWorkspace workspace)
            {
                workspace.Loaded += (_, _) => OnWorkspaceLoaded(workspace);
            }

            // 기능 동작 - Workspace 초기화 시
            private static void OnWorkspaceLoaded(ProjectEditWorkspace workspace)
            {
                if (Window.GetWindow(workspace) is not Window window) return;

                // 창을 닫을 때 최근 파일 목록 저장
                window.Closing += (sender, e) => { RecentFileHelper.Save(); };

                // 최근 파일 메뉴 동작 적용
                workspace.MenuFileRecent.SubmenuOpened += (_, _) =>
                {
                    RecentFileHelper.Populate(workspace.MenuFileRecent, (filePath) => OnRecentFileSubMenuClicked(workspace, filePath));
                };
            }

            private static void OnRecentFileSubMenuClicked(ProjectEditWorkspace workspace, string filePath)
            {
                workspace.SafeAction(() =>
                {
                    if (workspace.HandleSaveChanges()) return;

                    // clean all groups
                    workspace.GroupContainer.Children.Clear();
                    var project = JassProjectReader.Read(filePath);
                    workspace.projectEditFacade.MakeNewProject(filePath, project);
                    project.Groups.ToList().ForEach(group =>
                    {
                        ElemGroup elemGroup = new(group);
                        workspace.GroupContainer.Children.Add(elemGroup);
                    });
                    workspace.ViewportResetDelayed(true);

                    workspace.MessageText.Info($"{Path.GetFileName(filePath)} 프로젝트를 열었습니다.");
                });
            }
        }
    }
}
