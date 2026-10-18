---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/ProjectEditWorkspace.Menu.File.cs"
ref: HEAD
---

﻿using System.IO;
using System.Windows;
using vJassMainJBlueprint.Utils;
using vJassMainJBlueprint.V1.Model;
using vJassMainJBlueprint.V1.ModelFacade;
using vJassMainJBlueprint.V1.ModelHelper;
using vJassMainJBlueprint.V1.ProjectEditor.Elements;

namespace vJassMainJBlueprint.V1.ProjectEditor
{
    partial class ProjectEditWorkspace
    {
        // 새 프로젝트 생성 메서드
        private void OnMenuFileNewClick(object? sender, RoutedEventArgs? e)
        {
            SafeAction(() =>
            {
                if (HandleSaveChanges()) return;

                // clean all groups
                GroupContainer.Children.Clear();
                projectEditFacade.MakeNewProject();
                ViewportResetDelayed(true);

                MessageText.Info("새 프로젝트를 생성했습니다.");
            });
        }

        // 프로젝트 열기 메서드
        private void OnMenuFileOpenClick(object? sender, RoutedEventArgs? e)
        {
            SafeAction(() =>
            {
                if (HandleSaveChanges()) return;

                Optional<string>.Of(JassProjectOpenFileDialog.Show())
                .IfPresent(filePath =>
                {
                    // clean all groups
                    GroupContainer.Children.Clear();
                    var project = JassProjectReader.Read(filePath);
                    projectEditFacade.MakeNewProject(filePath, project);
                    project.Groups.ToList().ForEach(group =>
                    {
                        ElemGroup elemGroup = new(group);
                        GroupContainer.Children.Add(elemGroup);
                    });
                    ViewportResetDelayed(true);

                    MessageText.Info($"{Path.GetFileName(filePath)} 프로젝트를 열었습니다.");
                });
            });
        }

        // 프로젝트 닫기 메서드
        private void OnMenuFileCloseClick(object? sender, RoutedEventArgs? e)
        {
            SafeAction(() =>
            {
                if (HandleSaveChanges()) return;

                // clean all groups
                GroupContainer.Children.Clear();
                projectEditFacade.MakeNewProject();
                ViewportResetDelayed(true);

                MessageText.Info("프로젝트를 닫았습니다.");
            });
        }

        // 프로젝트 저장 메서드
        private void OnMenuFileSaveClick(object? sender, RoutedEventArgs? e)
        {
            SafeAction(() =>
            {
                Optional<bool>.Of(projectEditFacade.OriginSaveRequired)
                    .Filter(saveRequired => saveRequired)
                    .IfPresent(_ =>
                    {
                        if (HandleSaveProject()) return;

                        MessageText.Info("저장했습니다.");
                    });
            });
        }

        // 다른 이름으로 프로젝트 저장 메서드
        private void OnMenuFileSaveAsClick(object? sender, RoutedEventArgs? e)
        {
            SafeAction(() =>
            {
                Optional<string>.Of(JassProjectSaveFileDialog.Show())
                .IfPresent(filePath =>
                {
                    JassProject.Group[] groups = GroupContainer.Children.OfType<ElemGroup>().Select(group => group.ToModel()).ToArray();
                    JassProjectWriter.Write(projectEditFacade.GetProject(groups), filePath);
                    projectEditFacade.UpdateOrigin(filePath);

                    MessageText.Info($"{Path.GetFileName(filePath)} 파일로 저장했습니다.");
                });
            });
        }

        // 프로그램 종료 메서드
        private void OnMenuFileExitClick(object sender, RoutedEventArgs e)
        {
            // 이미 창 닫기 이벤트에서 저장 여부를 확인하므로 여기서는 확인하지 않음
            Application.Current.Shutdown();
        }

        // 변경사항 저장 여부 확인 및 처리 메서드
        private bool HandleSaveChanges()
        {
            if (projectEditFacade.OriginSaveRequired)
            {
                switch (MessageBox.Show(Window.GetWindow(this), "저장되지 않은 변경사항이 있습니다. 저장하시겠습니까?", "저장", MessageBoxButton.YesNoCancel, MessageBoxImage.Question))
                {
                    case MessageBoxResult.Cancel:
                        return true;
                    case MessageBoxResult.No:
                        break;
                    case MessageBoxResult.Yes:
                        HandleSaveProject();
                        break;
                }
            }
            return false;
        }

        // 프로젝트 저장 메서드
        private bool HandleSaveProject()
        {
            if (projectEditFacade.Origin == ProjectEditFacade.OriginType.File)
            {
                JassProject.Group[] groups = GroupContainer.Children.OfType<ElemGroup>().Select(group => group.ToModel()).ToArray();
                JassProjectWriter.Write(projectEditFacade.GetProject(groups), projectEditFacade.OriginFilePath);
                projectEditFacade.UpdateOriginSaveRequired(false);
                return false;
            }
            else
            {
                if (JassProjectSaveFileDialog.Show() is string filePath)
                {
                    JassProject.Group[] groups = GroupContainer.Children.OfType<ElemGroup>().Select(group => group.ToModel()).ToArray();
                    JassProjectWriter.Write(projectEditFacade.GetProject(groups), filePath);
                    projectEditFacade.UpdateOrigin(filePath);
                    return false;
                }
                return true;
            }
        }
    }
}
