---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "V1/ModelHelper/JassProjectReader.cs"
ref: HEAD
---

﻿using System.IO;
using System.Text.Json.Nodes;
using vJassMainJBlueprint.Utils;
using vJassMainJBlueprint.V1.Config;
using vJassMainJBlueprint.V1.Model;
using static vJassMainJBlueprint.V1.Model.JassProject;

namespace vJassMainJBlueprint.V1.ModelHelper
{
    internal class JassProjectReader
    {
        private static class VersionSpecification
        {
            public static readonly Dictionary<int, Func<JsonObject, JassProject>> VersionParseActions = new()
                {
                    { 24090800, Version24090800 },
                };

            private static JassProject Version24090800(JsonObject json)
            {
                return new JassProject(
                    LoadValue<string>(json, "name"),
                    LoadValue<string>(json, "author"),
                    LoadValue<int>(json, "width"),
                    LoadValue<int>(json, "height"),
                    LoadItems(json),
                    LoadGroups(json)
                );
            }

            private static Group[] LoadGroups(JsonObject json)
            {
                return Load<JsonArray>(json, "groups")
                    .Select(groupNode =>
                    {
                        if (groupNode is not JsonObject itemObject) { throw new Exception("Invalid item type."); }
                        return new Group(
                                LoadValue<int>(itemObject, "x"),
                                LoadValue<int>(itemObject, "y"),
                                LoadValue<int>(itemObject, "width"),
                                LoadValue<int>(itemObject, "height"),
                                LoadValue<string>(itemObject, "text"),
                                LoadValue<int>(itemObject, "fontSize"),
                                (GroupTextAlignment)Enum.Parse(typeof(GroupTextAlignment),LoadValue<string>(itemObject, "align")),
                                ColorHelper.Parse(LoadValue<string>(itemObject, "foreground")),
                                ColorHelper.Parse(LoadValue<string>(itemObject, "background"))
                            );
                    })
                    .ToArray();
            }

            private static Node[] LoadItems(JsonObject json)
            {
                return Load<JsonArray>(json, "items")
                    .Select(itemNode =>
                    {
                        if (itemNode is not JsonObject itemObject) { throw new Exception("Invalid item type."); }
                        return new Node(
                            LoadValue<int>(itemObject, "x"),
                            LoadValue<int>(itemObject, "y"),
                            LoadValue<int>(itemObject, "width"),
                            LoadValue<int>(itemObject, "height"),
                            LoadValue<string>(itemObject, "sourceFilePath"),
                            LoadImageBase64(itemObject)
                        );
                    })
                    .ToArray();
            }

            private static string? LoadImageBase64(JsonObject itemObject)
            {
                return Optional<JsonNode>.Of(itemObject["imageBase64"])
                    .Map(node => node.GetValue<string>())
                    .OrElse(null);
            }
        }

        public static JassProject Read(string filePath)
        {
            var json = ReadJsonFromFile(filePath);
            return ConvertWithVersion(json);
        }

        private static JassProject ConvertWithVersion(JsonObject json)
        {
            return Optional<int>.Of(LoadValue<int>(json, "version"))
                .Map(version =>
                {
                    return Optional<Dictionary<int, Func<JsonObject, JassProject>>>.Of(VersionSpecification.VersionParseActions)
                        .Filter(actions => actions.ContainsKey(version))
                        .Map(actions => actions[version].Invoke(json))
                        .OrElseThrow(new Exception($"Unsupported version: {version}"));
                })
                .GetValue();
        }

        private static T LoadValue<T>(JsonObject json, string key)
        {
            return Optional<JsonNode>.Of(json[key])
                .Map(node => node.GetValue<T>())
                .OrElseThrow(new Exception($"Essential data '{key}' was missing"));
        }

        private static T Load<T>(JsonNode node, string key)
        {
            return Optional<JsonNode>.Of(node[key])
                .Cast<T>()
                .OrElseThrow(new Exception($"Essential data '{key}' must be type of '{nameof(T)}'"));
        }

        private static JsonObject ReadJsonFromFile(string filePath)
        {
            var filteredLines = FilterJsonLines(filePath);
            var jsonString = string.Join(Environment.NewLine, filteredLines);
            return ParseJsonObject(jsonString);
        }

        private static List<string> FilterJsonLines(string filePath)
        {
            var filteredLines = new List<string>();
            using (var reader = new StreamReader(filePath))
            {
                string? line;
                while ((line = reader.ReadLine()) != null)
                {
                    Optional<string>.Of(line.Trim())
                        .Filter(line => line.StartsWith(JassProjectSpecification.JsonPrefix))
                        .Map(line => line[JassProjectSpecification.JsonPrefix.Length..])
                        .IfPresent(filteredLines.Add);
                }
            }
            return filteredLines;
        }

        private static JsonObject ParseJsonObject(string jsonString)
        {
            return Optional<string>.Of(jsonString)
                .Map(json => JsonNode.Parse(json))
                .Cast<JsonObject>()
                .OrElseThrow(new Exception("Failed to parse JSON."));
        }
    }
}
