---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ModelFacade/ProjectEditFacade.Event.cs"
ref: HEAD
---

﻿using System.Windows.Media.Imaging;

namespace vJassMainJBlueprint.V1.ModelFacade
{
    partial class ProjectEditFacade
    {
        public enum EventType
        {
            PROJECT_ORIGIN_UPDATE,
            PROJECT_ORIGIN_SAVE_REQUIRE,

            PROJECT_NAME_CHANGED,
            PROJECT_AUTHOR_CHANGED,
            PROJECT_RESIZE,

            NODE_ADD,
            NODE_REMOVE,
            NODE_BOUND_UPDATE,
            NODE_SOURCE_FILE_PATH_UPDATE,
            NODE_IMAGE_UPDATE,
        }

        public interface IUpdateRequiredEventArgs
        {
            EventType EventType { get; }
        }

        public readonly struct ProjectOriginUpdateEventArgs : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.PROJECT_ORIGIN_UPDATE;
            public OriginType OriginType { get; }
            public string OriginFilePath { get; }

            internal ProjectOriginUpdateEventArgs(OriginConfig originConfig) : this()
            {
                OriginType = originConfig.Origin;
                OriginFilePath = originConfig.OriginFilePath;
            }
        }

        public readonly struct ProjectOriginSaveRequireEventArgs : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.PROJECT_ORIGIN_SAVE_REQUIRE;
            public bool SaveRequired { get; }

            internal ProjectOriginSaveRequireEventArgs(OriginConfig originConfig) : this()
            {
                SaveRequired = originConfig.OriginSaveRequired;
            }
        }

        public readonly struct ProjectNameChangedEventArgs : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.PROJECT_NAME_CHANGED;
            public string Name { get; }

            internal ProjectNameChangedEventArgs(ProjectConfig projectConfig) : this()
            {
                Name = projectConfig.Name;
            }
        }

        public readonly struct ProjectAuthorChangedEventArgs : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.PROJECT_AUTHOR_CHANGED;
            public string Author { get; }

            internal ProjectAuthorChangedEventArgs(ProjectConfig projectConfig) : this()
            {
                Author = projectConfig.Author;
            }
        }

        public readonly struct ProjectResizeEventArgs : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.PROJECT_RESIZE;
            public int Width { get; }
            public int Height { get; }

            internal ProjectResizeEventArgs(ProjectConfig projectConfig) : this()
            {
                Width = projectConfig.Width;
                Height = projectConfig.Height;
            }
        }

        public readonly struct NodeAddEventArgs : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.NODE_ADD;
            public long NodeHandleId { get; }
            public int X { get; }
            public int Y { get; }
            public int Width { get; }
            public int Height { get; }
            public string SourceFilePath { get; }
            public BitmapImage? Image { get; }

            internal NodeAddEventArgs(NodeConfigEntity nodeConfig)
            {
                NodeHandleId = nodeConfig.NodeHandleId;
                X = nodeConfig.X;
                Y = nodeConfig.Y;
                Width = nodeConfig.Width;
                Height = nodeConfig.Height;
                SourceFilePath = nodeConfig.SourceFilePath;
                Image = nodeConfig.Image;
            }
        }

        public readonly struct NodeRemoveEventArgs(long nodeHandleId) : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.NODE_REMOVE;
            public long NodeHandleId { get; } = nodeHandleId;
        }

        public readonly struct NodeBoundUpdateEventArgs : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.NODE_BOUND_UPDATE;
            public long NodeHandleId { get; }
            public int X { get; }
            public int Y { get; }
            public int Width { get; }
            public int Height { get; }

            internal NodeBoundUpdateEventArgs(NodeConfigEntity nodeConfig)
            {
                NodeHandleId = nodeConfig.NodeHandleId;
                X = nodeConfig.X;
                Y = nodeConfig.Y;
                Width = nodeConfig.Width;
                Height = nodeConfig.Height;
            }
        }

        public readonly struct NodeImageUpdateEventArgs : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.NODE_IMAGE_UPDATE;
            public long NodeHandleId { get; }
            public BitmapImage? Image { get; }

            internal NodeImageUpdateEventArgs(NodeConfigEntity nodeConfig)
            {
                NodeHandleId = nodeConfig.NodeHandleId;
                Image = nodeConfig.Image;
            }
        }

        public readonly struct NodeSourceFilePathUpdateEventArgs : IUpdateRequiredEventArgs
        {
            public EventType EventType { get; } = EventType.NODE_SOURCE_FILE_PATH_UPDATE;
            public long NodeHandleId { get; }
            public string SourceFilePath { get; }

            internal NodeSourceFilePathUpdateEventArgs(NodeConfigEntity nodeConfig)
            {
                NodeHandleId = nodeConfig.NodeHandleId;
                SourceFilePath = nodeConfig.SourceFilePath;
            }
        }

        public event UpdateRequiredEventHandler? UpdateRequired;

        public delegate void UpdateRequiredEventHandler(ProjectEditFacade sender, IUpdateRequiredEventArgs[] e);
    }
}
