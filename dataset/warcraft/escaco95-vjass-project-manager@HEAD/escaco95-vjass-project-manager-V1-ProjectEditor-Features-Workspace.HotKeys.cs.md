---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/Features/Workspace.HotKeys.cs"
ref: HEAD
---

﻿using System.Windows.Input;
using vJassMainJBlueprint.Utils;

namespace vJassMainJBlueprint.V1.ProjectEditor
{
    partial class ProjectEditWorkspace
    {
        [FeatureManager.FeatureSubscriber]
        private class FeatureWorkspaceHotKeys
        {
            private record KeyBindingSettings(string Key, ModifierKeys Modifiers);

            // 키 설정을 저장할 딕셔너리
            private static readonly Dictionary<string, KeyBindingSettings> keyBindings = new()
            {
                { "file.new", new KeyBindingSettings("N",ModifierKeys.Control) },
                { "file.open", new KeyBindingSettings("O", ModifierKeys.Control) },
                { "file.close", new KeyBindingSettings("W", ModifierKeys.Control) },
                { "file.save", new KeyBindingSettings("S", ModifierKeys.Control) },
                { "file.saveas", new KeyBindingSettings("S", ModifierKeys.Control | ModifierKeys.Shift) },
                { "edit.delete", new KeyBindingSettings("Delete", ModifierKeys.None) },
                { "edit.selectall", new KeyBindingSettings("A", ModifierKeys.Control) },
                { "edit.unselect", new KeyBindingSettings("Escape", ModifierKeys.None) },
                { "edit.copy.nodeimage", new KeyBindingSettings("C", ModifierKeys.Control | ModifierKeys.Shift) },
                { "edit.paster.nodeimage", new KeyBindingSettings("V", ModifierKeys.Control | ModifierKeys.Shift) },
                { "view.tool.sampleicon",  new KeyBindingSettings("I", ModifierKeys.Control | ModifierKeys.Alt) },
                { "view.allnodes",  new KeyBindingSettings("Space", ModifierKeys.None) },
                { "project.source.add",  new KeyBindingSettings("A", ModifierKeys.Control | ModifierKeys.Shift) },
                { "project.source.adddir",  new KeyBindingSettings("D", ModifierKeys.Control | ModifierKeys.Shift) },
                { "project.source.addgroup",  new KeyBindingSettings("G", ModifierKeys.Control | ModifierKeys.Shift) },
            };

            // 기능 초기화
            private FeatureWorkspaceHotKeys()
            {
                FeatureManager.SubscribeToFeature<ProjectEditWorkspace>(InitializeFeature);
            }

            // 기능 적용
            private static void InitializeFeature(ProjectEditWorkspace workspace)
            {
                workspace.KeyDown += (_, e) => OnWorkspaceKeyDown(workspace, e);
            }

            // 기능 동작 - Workspace 키 다운 시
            private static void OnWorkspaceKeyDown(ProjectEditWorkspace workspace, KeyEventArgs e)
            {
                // 현재 키와 수정자 확인
                var modifiers = Keyboard.Modifiers;
                var key = e.Key;

                foreach (var binding in keyBindings)
                {
                    var bindingKey = (Key)Enum.Parse(typeof(Key), binding.Value.Key);
                    if (bindingKey == key && binding.Value.Modifiers == modifiers)
                    {
                        // 매핑된 명령 실행
                        ExecuteCommand(workspace, binding.Key);
                        e.Handled = true; // 이벤트 처리 완료
                        return;
                    }
                }

                // 기본 처리 호출
                workspace.OnKeyDown(e);
            }

            private static void ExecuteCommand(ProjectEditWorkspace workspace, string command)
            {
                switch (command)
                {
                    case "file.new":
                        workspace.OnMenuFileNewClick(null, null);
                        break;
                    case "file.open":
                        workspace.OnMenuFileOpenClick(null, null);
                        break;
                    case "file.close":
                        workspace.OnMenuFileCloseClick(null, null);
                        break;
                    case "file.save":
                        workspace.OnMenuFileSaveClick(null, null);
                        break;
                    case "file.saveas":
                        workspace.OnMenuFileSaveAsClick(null, null);
                        break;
                    case "edit.delete":
                        workspace.OnMenuEditDelete(null, null);
                        break;
                    case "edit.selectall":
                        workspace.OnMenuEditSelectAll(null, null);
                        break;
                    case "edit.unselect":
                        workspace.OnMenuEditUnselectAll(null, null);
                        break;
                    case "edit.copy.nodeimage":
                        workspace.OnMenuEditCopyImage(null, null);
                        break;
                    case "edit.paster.nodeimage":
                        workspace.OnMenuEditPasteImage(null, null);
                        break;
                    case "view.tool.sampleicon":
                        workspace.OnMenuViewToolSampleIcon(null, null);
                        break;
                    case "view.allnodes":
                        workspace.OnMenuViewAllNodesClick(null, null);
                        break;
                    case "project.source.add":
                        workspace.OnMenuProjectSourceAdd(null, null);
                        break;
                    case "project.source.adddir":
                        workspace.OnMenuProjectSourceAddDirectory(null, null);
                        break;
                    case "project.source.addgroup":
                        workspace.OnMenuProjectGroupAdd(null, null);
                        break;
                }
            }
        }
    }
}
