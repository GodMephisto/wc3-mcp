---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ModelFacade/ProjectEditFacade.cs"
ref: HEAD
---

﻿using System.Windows.Media.Imaging;
using vJassMainJBlueprint.Utils;
using vJassMainJBlueprint.V1.Config;
using vJassMainJBlueprint.V1.Model;
using static vJassMainJBlueprint.V1.Model.JassProject;

namespace vJassMainJBlueprint.V1.ModelFacade
{
    public partial class ProjectEditFacade
    {
        private readonly OriginConfig _originConfig = new();
        private readonly ProjectConfig _projectConfig = new();
        private readonly NodeCollectionConfig _nodeCollectionConfigs = new();

        public enum OriginType
        {
            None,
            Memory,
            File,
        }

        internal class OriginConfig
        {
            public OriginType Origin = OriginType.Memory;
            public string OriginFilePath = string.Empty;
            public bool OriginSaveRequired = false;

            public void Reset()
            {
                Origin = OriginType.Memory;
                OriginFilePath = string.Empty;
                OriginSaveRequired = false;
            }

            public void ResetToFile(string filePath)
            {
                Origin = OriginType.File;
                OriginFilePath = filePath;
                OriginSaveRequired = false;
            }
        }

        internal class ProjectConfig
        {
            public string Name = JassProjectSpecification.DefaultProjectName;
            public string Author = JassProjectSpecification.DefaultProjectAuthor;
            public int Width = JassProjectSpecification.DefaultProjectWidth;
            public int Height = JassProjectSpecification.DefaultProjectHeight;

            public void Reset()
            {
                Name = JassProjectSpecification.DefaultProjectName;
                Author = JassProjectSpecification.DefaultProjectAuthor;
                Width = JassProjectSpecification.DefaultProjectWidth;
                Height = JassProjectSpecification.DefaultProjectHeight;
            }

            public void Consume(JassProject jassProject)
            {
                Name = jassProject.Name;
                Author = jassProject.Author;
                Width = jassProject.Width;
                Height = jassProject.Height;
            }

            public JassProject Cast(Node[] nodes, Group[] groups)
            {
                return new JassProject(Name, Author, Width, Height, nodes, groups);
            }
        }

        internal class NodeCollectionConfig
        {
            public long NodeHandleId = 0x100000;
            public Dictionary<long, NodeConfigEntity> NodeConfigs = [];
            public Dictionary<string, List<NodeConfigEntity>> IndexNodeSourceFile = [];

            public void Reset()
            {
                NodeHandleId = 0x100000;
                NodeConfigs.Clear();
                IndexNodeSourceFile.Clear();
            }

            public void Consume(JassProject jassProject)
            {
                Reset();

                foreach (var node in jassProject.Nodes)
                {
                    NodeConfigEntity nodeConfig = new(NodeHandleId, node);
                    NodeConfigs.Add(NodeHandleId, nodeConfig);
                    Index(nodeConfig);
                    NodeHandleId++;
                }
            }

            public List<NodeConfigEntity> Insert(List<NodeAddRequest> requests)
            {
                return requests.Select(request =>
                {
                    NodeConfigEntity nodeConfig = new(NodeHandleId, request);
                    NodeConfigs.Add(NodeHandleId, nodeConfig);
                    Index(nodeConfig);
                    NodeHandleId++;
                    return nodeConfig;
                }).ToList();
            }

            public List<long> Remove(List<long> nodeHandleIds)
            {
                return nodeHandleIds.Where(nodeHandleId =>
                {
                    if (!NodeConfigs.TryGetValue(nodeHandleId, out var nodeConfig)) return false;
                    Deindex(nodeConfig);
                    NodeConfigs.Remove(nodeConfig.NodeHandleId);
                    return true;
                }).ToList();
            }

            public List<long> Select(List<long> nodeHandleIds)
            {
                return nodeHandleIds.Where(NodeConfigs.ContainsKey).ToList();
            }

            public List<T> Select<T>(List<T> selectRequests) where T : INodeSelectRequest
            {
                return selectRequests.Where(selectReqeust => NodeConfigs.ContainsKey(selectReqeust.NodeHandleId)).ToList();
            }

            public List<NodeConfig> SelectAllBySourceFilePathIn(List<string> sourceFilePaths)
            {
                var selectedNodes = new List<NodeConfig>();

                foreach (var sourceFilePath in sourceFilePaths.Distinct())
                {
                    if (!IndexNodeSourceFile.TryGetValue(sourceFilePath, out var nodeConfigs)) continue;
                    selectedNodes.AddRange(nodeConfigs.Select(nodeConfig => nodeConfig.Snapshot()));
                }
                return selectedNodes;
            }

            public void Index(NodeConfigEntity nodeConfig)
            {
                if (!IndexNodeSourceFile.TryGetValue(nodeConfig.SourceFilePath, out var nodeConfigs))
                {
                    nodeConfigs = [];
                    IndexNodeSourceFile.Add(nodeConfig.SourceFilePath, nodeConfigs);
                }
                nodeConfigs.Add(nodeConfig);
            }

            public void Deindex(NodeConfigEntity nodeConfig)
            {
                var sourceFilePath = nodeConfig.SourceFilePath;
                var nodeConfigs = IndexNodeSourceFile[sourceFilePath];
                nodeConfigs.Remove(nodeConfig);
                if (nodeConfigs.Count == 0)
                {
                    IndexNodeSourceFile.Remove(sourceFilePath);
                }
            }
        }

        public class NodeConfig(long nodeHandleId, int x, int y, int width, int height, string sourceFilePath, BitmapImage? image)
        {
            public long NodeHandleId { get; private set; } = nodeHandleId;
            public int X { get; private set; } = x;
            public int Y { get; private set; } = y;
            public int Width { get; private set; } = width;
            public int Height { get; private set; } = height;
            public string SourceFilePath { get; private set; } = sourceFilePath;
            public BitmapImage? Image { get; private set; } = image;
        }

        internal class NodeConfigEntity
        {
            public long NodeHandleId;
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public string SourceFilePath;
            public BitmapImage? Image;

            internal NodeConfigEntity(long nodeHandleId, NodeAddRequest request)
            {
                NodeHandleId = nodeHandleId;
                X = request.X;
                Y = request.Y;
                Width = request.Width;
                Height = request.Height;
                SourceFilePath = request.SourceFilePath;
                Image = request.Image;
            }

            internal NodeConfigEntity(long nodeHandleId, Node node)
            {
                NodeHandleId = nodeHandleId;
                X = node.X;
                Y = node.Y;
                Width = node.Width;
                Height = node.Height;
                SourceFilePath = node.SourceFilePath;
                Image = Base64Helper.Convert(node.ImageBase64String);
            }

            public JassProject.Node Cast()
            {
                return new JassProject.Node(X, Y, Width, Height, SourceFilePath, Base64Helper.Convert(Image));
            }

            public NodeConfig Snapshot()
            {
                return new NodeConfig(NodeHandleId, X, Y, Width, Height, SourceFilePath, Image);
            }
        }
    }
}
