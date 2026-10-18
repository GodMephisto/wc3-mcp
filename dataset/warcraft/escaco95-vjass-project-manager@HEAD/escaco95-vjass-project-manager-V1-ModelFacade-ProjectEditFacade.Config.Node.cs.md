---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ModelFacade/ProjectEditFacade.Config.Node.cs"
ref: HEAD
---

﻿using System.Windows.Media.Imaging;

namespace vJassMainJBlueprint.V1.ModelFacade
{
    partial class ProjectEditFacade
    {
        public List<NodeConfig> SelectNodeBySourceFilePath(string sourceFilePath) => _nodeCollectionConfigs.SelectAllBySourceFilePathIn([sourceFilePath]);

        public List<NodeConfig> SelectAllNodeBySourceFilePathIn(List<string> sourceFilePaths) => _nodeCollectionConfigs.SelectAllBySourceFilePathIn(sourceFilePaths);

        public string? SelectNodeSourceFilePath(long? nodeHandleId)
        {
            if (!nodeHandleId.HasValue) return null;
            if (!_nodeCollectionConfigs.NodeConfigs.TryGetValue(nodeHandleId.Value, out NodeConfigEntity? nodeConfig)) return null;

            return nodeConfig.SourceFilePath;
        }

        public List<string> SelectNodeSourceFilePaths() => [.. _nodeCollectionConfigs.IndexNodeSourceFile.Keys];

        public BitmapImage? SelectNodeImage(long? nodeHandleId)
        {
            if (!nodeHandleId.HasValue) return null;
            if (!_nodeCollectionConfigs.NodeConfigs.TryGetValue(nodeHandleId.Value, out NodeConfigEntity? nodeConfig)) return null;

            return nodeConfig.Image;
        }

        public void DeleteNode(long nodeHandleId) => DeleteNodes([nodeHandleId]);

        public void DeleteNodes(List<long> nodeHandleIds)
        {
            List<NodeRemoveEventArgs> deletedNodeEventArgs = _nodeCollectionConfigs.Remove(nodeHandleIds)
                    .Select(nodeHandleId => new NodeRemoveEventArgs(nodeHandleId)).ToList();

            if (deletedNodeEventArgs.Count == 0) return;

            UpdateRequired?.Invoke(this, [.. deletedNodeEventArgs]);
        }

        public interface INodeSelectRequest
        {
            long NodeHandleId { get; }
        }

        public struct NodePositionUpdateRequest : INodeSelectRequest
        {
            public long NodeHandleId { get; set; }
            public int X;
            public int Y;
        }

        public void UpdateNodePosition(NodePositionUpdateRequest request) => UpdateNodePosition([request]);

        public void UpdateNodePosition(List<NodePositionUpdateRequest> requests)
        {
            UpdateNode(requests, (request) =>
            {
                NodeConfigEntity selectedNodeConfig = _nodeCollectionConfigs.NodeConfigs[request.NodeHandleId];

                selectedNodeConfig.X = request.X;
                selectedNodeConfig.Y = request.Y;

                return new NodeImageUpdateEventArgs(selectedNodeConfig);
            });
        }

        public struct NodeSizeUpdateRequest : INodeSelectRequest
        {
            public long NodeHandleId { get; set; }
            public int Width;
            public int Height;
        }

        public void UpdateNodeSize(NodeSizeUpdateRequest request) => UpdateNodeSize([request]);

        public void UpdateNodeSize(List<NodeSizeUpdateRequest> requests)
        {
            UpdateNode(requests, (request) =>
            {
                NodeConfigEntity selectedNodeConfig = _nodeCollectionConfigs.NodeConfigs[request.NodeHandleId];

                selectedNodeConfig.Width = request.Width;
                selectedNodeConfig.Height = request.Height;

                return new NodeImageUpdateEventArgs(selectedNodeConfig);
            });
        }

        public struct NodeImageUpdateRequest : INodeSelectRequest
        {
            public long NodeHandleId { get; set; }
            public BitmapImage? Image;
        }

        public void UpdateNodeImage(NodeImageUpdateRequest request) => UpdateNodeImage([request]);

        public void UpdateNodeImage(List<NodeImageUpdateRequest> requests)
        {
            UpdateNode(requests, (request) =>
            {
                NodeConfigEntity selectedNodeConfig = _nodeCollectionConfigs.NodeConfigs[request.NodeHandleId];

                selectedNodeConfig.Image = request.Image;

                return new NodeImageUpdateEventArgs(selectedNodeConfig);
            });
        }

        public void UpdateNodeImage(long nodeHandleId, BitmapImage? image) => UpdateNodeImage([nodeHandleId], image);

        public void UpdateNodeImage(List<long> nodeHandleIds, BitmapImage? image)
        {
            UpdateNode(nodeHandleIds, (nodeHandleId) =>
            {
                NodeConfigEntity selectedNodeConfig = _nodeCollectionConfigs.NodeConfigs[nodeHandleId];

                selectedNodeConfig.Image = image;

                return new NodeImageUpdateEventArgs(selectedNodeConfig);
            });
        }

        public struct NodeSourceFilePathUpdateRequest : INodeSelectRequest
        {
            public long NodeHandleId { get; set; }
            public string SourceFilePath;
        }

        public int UpdateNodeSourceFilePath(NodeSourceFilePathUpdateRequest request) => UpdateNodeSourceFilePath([request]);

        public int UpdateNodeSourceFilePath(List<NodeSourceFilePathUpdateRequest> requests)
        {
            return UpdateNode(requests, (request) =>
            {
                NodeConfigEntity selectedNodeConfig = _nodeCollectionConfigs.NodeConfigs[request.NodeHandleId];

                _nodeCollectionConfigs.Deindex(selectedNodeConfig);
                selectedNodeConfig.SourceFilePath = request.SourceFilePath;
                _nodeCollectionConfigs.Index(selectedNodeConfig);

                return new NodeSourceFilePathUpdateEventArgs(selectedNodeConfig);
            });
        }

        private int UpdateNode<A>(List<long> nodeHandleIds, Func<long, A> requestFunc) where A : IUpdateRequiredEventArgs
        {
            List<long> selectedNodeIds = _nodeCollectionConfigs.Select(nodeHandleIds);

            if (selectedNodeIds.Count == 0) return 0;

            List<A> updateEventArgs = selectedNodeIds.Select(requestFunc.Invoke).ToList();

            UpdateRequired?.Invoke(this, [.. updateEventArgs]);

            return updateEventArgs.Count;
        }

        private int UpdateNode<T, A>(List<T> requests, Func<T, A> requestFunc) where T : INodeSelectRequest where A : IUpdateRequiredEventArgs
        {
            List<T> selectedReqeusts = _nodeCollectionConfigs.Select(requests);

            if (selectedReqeusts.Count == 0) return 0;

            List<A> updateEventArgs = selectedReqeusts.Select(requestFunc.Invoke).ToList();

            UpdateRequired?.Invoke(this, [.. updateEventArgs]);

            return updateEventArgs.Count;
        }

        public struct NodeAddRequest
        {
            public int X;
            public int Y;
            public int Width;
            public int Height;
            public string SourceFilePath;
            public BitmapImage? Image;
        }

        public int InsertNode(List<NodeAddRequest> requests)
        {
            List<NodeConfigEntity> nodeConfigs = _nodeCollectionConfigs.Insert(requests);

            List<NodeAddEventArgs> updateRequiredEventArgs = nodeConfigs.Select(nodeConfig => new NodeAddEventArgs(nodeConfig)).ToList();

            UpdateRequired?.Invoke(this, [.. updateRequiredEventArgs]);

            return nodeConfigs.Count;
        }
    }
}
