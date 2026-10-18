---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/Model/JassProject.cs"
ref: HEAD
---

﻿using System.Windows.Media;
using vJassMainJBlueprint.V1.Config;

namespace vJassMainJBlueprint.V1.Model
{
    public readonly record struct JassProject(string Name, string Author, int Width, int Height, JassProject.Node[] Nodes, JassProject.Group[] Groups)
    {
        /// <summary>
        /// 프로젝트의 기본값입니다.
        /// </summary>
        public static readonly JassProject Default = new(
            JassProjectSpecification.DefaultProjectName,
            JassProjectSpecification.DefaultProjectAuthor,
            JassProjectSpecification.DefaultProjectWidth,
            JassProjectSpecification.DefaultProjectHeight,
            [],
            []);

        public readonly string Name { get; } = Name;
        public readonly string Author { get; } = Author;
        public readonly int Width { get; } = Width;
        public readonly int Height { get; } = Height;
        public readonly Node[] Nodes { get; } = Nodes;
        public readonly Group[] Groups { get; } = Groups;

        public readonly record struct Node(int X, int Y, int Width, int Height, string SourceFilePath, string? ImageBase64String)
        {
            public readonly int X { get; } = X;
            public readonly int Y { get; } = Y;
            public readonly int Width { get; } = Width;
            public readonly int Height { get; } = Height;
            public readonly string SourceFilePath { get; } = SourceFilePath;
            public readonly string? ImageBase64String { get; } = ImageBase64String;
        }

        public readonly record struct Group(int X, int Y, int Width, int Height, string Text, int FontSize, GroupTextAlignment align, Color Foreground, Color Background)
        {
            public readonly int X { get; } = X;
            public readonly int Y { get; } = Y;
            public readonly int Width { get; } = Width;
            public readonly int Height { get; } = Height;
            public readonly string Text { get; } = Text;
            public readonly int FontSize { get; } = FontSize;
            public readonly GroupTextAlignment Align { get; } = align;
            public readonly Color Foreground { get; } = Foreground;
            public readonly Color Background { get; } = Background;
        }

        public enum GroupTextAlignment
        {
            TopLeft,
            TopCenter,
            TopRight,
            MiddleLeft,
            MiddleCenter,
            MiddleRight,
            BottomLeft,
            BottomCenter,
            BottomRight
        }
    }
}
