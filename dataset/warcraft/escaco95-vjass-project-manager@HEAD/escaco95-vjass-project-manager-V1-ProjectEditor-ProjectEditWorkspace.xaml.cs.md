---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ProjectEditor/ProjectEditWorkspace.xaml.cs"
ref: HEAD
---

﻿using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using vJassMainJBlueprint.Utils;
using vJassMainJBlueprint.V1.ModelFacade;
using vJassMainJBlueprint.V1.ProjectEditor.Elements;

namespace vJassMainJBlueprint.V1.ProjectEditor
{
    /// <summary>
    /// ProjectEditWorkspace.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ProjectEditWorkspace : UserControl
    {
        // 편집기 파사드
        private readonly ProjectEditFacade projectEditFacade = new();
        private readonly Dictionary<long, ElemNode> nodeElements = [];

        public ProjectEditWorkspace()
        {
            InitializeComponent();

            // FeatureManager를 통해 기능 초기화
            this.InitializeFeatures();

            // 편집기 파사드 초기화
            projectEditFacade.UpdateRequired += OnEditFacadeUpdateRequired;
        }

        private void OnEditFacadeUpdateRequired(ProjectEditFacade sender, ProjectEditFacade.IUpdateRequiredEventArgs[] args)
        {
            foreach (var arg in args)
            {
                switch (arg.EventType)
                {
                    case ProjectEditFacade.EventType.PROJECT_ORIGIN_UPDATE:
                        OnProjectOriginChanged((ProjectEditFacade.ProjectOriginUpdateEventArgs)arg);
                        break;
                    case ProjectEditFacade.EventType.PROJECT_ORIGIN_SAVE_REQUIRE:
                        OnProjectSaveRequiredChanged((ProjectEditFacade.ProjectOriginSaveRequireEventArgs)arg);
                        break;
                    case ProjectEditFacade.EventType.PROJECT_NAME_CHANGED:
                        OnProjectNameChanged((ProjectEditFacade.ProjectNameChangedEventArgs)arg);
                        break;
                    case ProjectEditFacade.EventType.PROJECT_AUTHOR_CHANGED:
                        OnProjectAuthorChanged((ProjectEditFacade.ProjectAuthorChangedEventArgs)arg);
                        break;
                    case ProjectEditFacade.EventType.PROJECT_RESIZE:
                        OnProjectSizeChanged((ProjectEditFacade.ProjectResizeEventArgs)arg);
                        break;
                    case ProjectEditFacade.EventType.NODE_ADD:
                        OnProjectNodeAdded((ProjectEditFacade.NodeAddEventArgs)arg);
                        break;
                    case ProjectEditFacade.EventType.NODE_REMOVE:
                        OnProjectNodeRemoved((ProjectEditFacade.NodeRemoveEventArgs)arg);
                        break;
                    case ProjectEditFacade.EventType.NODE_BOUND_UPDATE:
                        OnProjectNodeBoundChanged((ProjectEditFacade.NodeBoundUpdateEventArgs)arg);
                        break;
                    case ProjectEditFacade.EventType.NODE_SOURCE_FILE_PATH_UPDATE:
                        OnProjectNodeSourceFilePathChanged((ProjectEditFacade.NodeSourceFilePathUpdateEventArgs)arg);
                        break;
                    case ProjectEditFacade.EventType.NODE_IMAGE_UPDATE:
                        OnProjectNodeImageChanged((ProjectEditFacade.NodeImageUpdateEventArgs)arg);
                        break;
                }
            }
        }

        private void OnProjectOriginChanged(ProjectEditFacade.ProjectOriginUpdateEventArgs e)
        {
            Optional<Window>.Of(Window.GetWindow(this)).IfPresent(window =>
            {
                if (window.Title.Contains('*'))
                {
                    window.Title = $"{(e.OriginType == ProjectEditFacade.OriginType.Memory ? "새 프로젝트" : Path.GetFileName(projectEditFacade.OriginFilePath))}* - vJass Project Manager";
                }
                else
                {
                    window.Title = $"{(e.OriginType == ProjectEditFacade.OriginType.Memory ? "새 프로젝트" : Path.GetFileName(projectEditFacade.OriginFilePath))} - vJass Project Manager";
                }
            });

            if (e.OriginType == ProjectEditFacade.OriginType.File)
            {
                RecentFileHelper.Touch(e.OriginFilePath);
            }
        }

        private void OnProjectSaveRequiredChanged(ProjectEditFacade.ProjectOriginSaveRequireEventArgs e)
        {
            Optional<Window>.Of(Window.GetWindow(this)).IfPresent(window =>
            {
                if (e.SaveRequired)
                {
                    window.Title = window.Title.Replace(" - vJass Project Manager", "* - vJass Project Manager");
                }
                else
                {
                    window.Title = window.Title.Replace("* - vJass Project Manager", " - vJass Project Manager");
                }
            });
        }

        private void OnProjectNameChanged(ProjectEditFacade.ProjectNameChangedEventArgs arg)
        {
            Dispatcher.Invoke(() =>
            {
                TitleBlockProject.ProjectName = arg.Name;
            });
        }

        private void OnProjectAuthorChanged(ProjectEditFacade.ProjectAuthorChangedEventArgs arg)
        {
            Dispatcher.Invoke(() =>
            {
                TitleBlockProject.ProjectAuthor = arg.Author;
            });
        }

        private void OnProjectSizeChanged(ProjectEditFacade.ProjectResizeEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                FooterZoomGrid.Width = e.Width;
                FooterZoomGrid.Height = e.Height;
                TextBlockProjectWidth.Text = $"{e.Width}";
                TextBlockProjectHeight.Text = $"{e.Height}";
            });
        }

        private void OnProjectNodeBoundChanged(ProjectEditFacade.NodeBoundUpdateEventArgs e)
        {
            Dispatcher.Invoke(() => { nodeElements[e.NodeHandleId].UpdateNode(e); });
        }

        private void OnProjectNodeSourceFilePathChanged(ProjectEditFacade.NodeSourceFilePathUpdateEventArgs e)
        {
            Dispatcher.Invoke(() => { nodeElements[e.NodeHandleId].UpdateNode(e); });
        }

        private void OnProjectNodeImageChanged(ProjectEditFacade.NodeImageUpdateEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                Dispatcher.Invoke(() => { nodeElements[e.NodeHandleId].UpdateNode(e); });
            });
        }

        private void OnProjectNodeRemoved(ProjectEditFacade.NodeRemoveEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                // 물리 노드 제거
                NodeContainer.Children.Remove(nodeElements[e.NodeHandleId]);
                // 논리 노드 제거
                nodeElements.Remove(e.NodeHandleId);
            });
        }

        private void OnProjectNodeAdded(ProjectEditFacade.NodeAddEventArgs e)
        {
            Dispatcher.Invoke(() =>
            {
                ElemNode nodeElement = new(e);
                // 논리 노드 추가
                nodeElements.Add(e.NodeHandleId, nodeElement);
                // 물리 노드 추가
                NodeContainer.Children.Add(nodeElement);
            });
        }

        public readonly struct SaveRequireMessage { }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            // 내부 Element 로부터 저장 필요함 메시지 수신
            Messenger.Subscribe<SaveRequireMessage>((message) => { Dispatcher.Invoke(() => projectEditFacade.UpdateOriginSaveRequired(true)); });

            // UserControl이 로드될 때 포커스 설정
            Keyboard.Focus(this);

            // 편집기 파사드 기본 동작 - 새 문서 생성
            projectEditFacade.MakeNewProject();
            // 부모 윈도우 닫기 동작 후킹
            Optional<Window>.Of(Window.GetWindow(this)).IfPresent(window =>
            {
                window.Closing += (sender, e) =>
                {
                    SafeAction(() =>
                    {
                        if (HandleSaveChanges())
                        {
                            e.Cancel = true;
                            return;
                        }

                        // clean all groups
                        GroupContainer.Children.Clear();
                        projectEditFacade.MakeNewProject();
                    });
                };
            });

            // 화면 초기화
            ViewportResetDelayed(true);
        }

        private void SafeAction(Action action)
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                MessageBox.Show(Window.GetWindow(this), ex.Message, "오류", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
