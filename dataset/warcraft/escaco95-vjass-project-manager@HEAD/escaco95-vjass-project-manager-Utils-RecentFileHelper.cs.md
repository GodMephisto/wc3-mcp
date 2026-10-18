---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "Utils/RecentFileHelper.cs"
ref: HEAD
---

﻿using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Windows.Controls;

namespace vJassMainJBlueprint.Utils
{
    /// <summary>
    /// 최근 파일 목록을 관리하고, MenuItem에 최근 파일 목록을 표시하는 헬퍼 클래스.
    /// </summary>
    internal class RecentFileHelper
    {
        private const int MaxRecentFiles = 10;
        private static readonly string RecentFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.recent.json");
        private static readonly string TempFilePath = RecentFilePath + ".temp";
        private static readonly List<string> CachedRecentFiles = [];
        private static bool IsCached = false;

        /// <summary>
        /// 최근 파일 목록에 파일을 추가하고, 기존에 있으면 순서를 갱신합니다.
        /// </summary>
        public static void Touch(string filePath)
        {
            CacheRecentFiles();

            // 파일이 존재하면 제거하고, 최신 파일을 첫 번째에 추가
            CachedRecentFiles.Remove(filePath);
            CachedRecentFiles.Insert(0, filePath);

            // 최대 파일 개수를 넘으면 마지막 항목 삭제
            if (CachedRecentFiles.Count > MaxRecentFiles)
            {
                CachedRecentFiles.RemoveAt(CachedRecentFiles.Count - 1);
            }
        }

        /// <summary>
        /// 최근 파일 목록을 JSON 파일로 저장합니다.
        /// </summary>
        public static void Save()
        {
            // 캐싱된 파일이 없다면 저장하지 않음
            if (!IsCached) return;

            try
            {
                var json = JsonSerializer.Serialize(CachedRecentFiles);
                File.WriteAllText(TempFilePath, json);

                if (File.Exists(RecentFilePath))
                {
                    File.Replace(TempFilePath, RecentFilePath, null);
                }
                else
                {
                    File.Move(TempFilePath, RecentFilePath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to save recent files: {ex.Message}");
            }
            finally
            {
                // 임시 파일 삭제
                if (File.Exists(TempFilePath))
                {
                    File.Delete(TempFilePath);
                }
            }
        }

        /// <summary>
        /// 최근 파일 목록을 MenuItem에 추가합니다.
        /// </summary>
        public static void Populate(MenuItem parentMenuItem, Action<string> childClickAction)
        {
            CacheRecentFiles();

            parentMenuItem.Items.Clear();

            if (CachedRecentFiles.Count == 0)
            {
                // 최근 파일이 없을 경우 "항목 없음"을 추가
                parentMenuItem.Items.Add(CreateNoItemsMenuItem());
            }
            else
            {
                int index = 1;
                foreach (var recentFile in CachedRecentFiles)
                {
                    var menuItem = CreateRecentFileMenuItem(recentFile, index, childClickAction);
                    parentMenuItem.Items.Add(menuItem);
                    index++;
                }
            }
        }

        /// <summary>
        /// "항목 없음"을 나타내는 비활성화된 MenuItem을 생성합니다.
        /// </summary>
        private static MenuItem CreateNoItemsMenuItem()
        {
            return new MenuItem
            {
                Header = "(항목 없음)",
                IsEnabled = false
            };
        }

        /// <summary>
        /// 최근 파일 항목을 나타내는 MenuItem을 생성합니다.
        /// </summary>
        private static MenuItem CreateRecentFileMenuItem(string filePath, int index, Action<string> childClickAction)
        {
            var menuItem = new MenuItem
            {
                Header = $"{index} {Path.GetFileName(filePath)} ({Path.GetDirectoryName(filePath)})",
            };

            // 클릭 시 파일 경로 전달
            menuItem.Click += (sender, e) =>
            {
                childClickAction(filePath);
            };

            return menuItem;
        }

        /// <summary>
        /// 최근 파일 목록을 캐싱하여 메모리에 저장합니다.
        /// </summary>
        private static void CacheRecentFiles()
        {
            if (IsCached) return;

            if (!File.Exists(RecentFilePath))
            {
                IsCached = true;
                return;
            }

            try
            {
                var json = File.ReadAllText(RecentFilePath);

                // JSON 파일에서 최근 파일 목록을 로드
                if (JsonSerializer.Deserialize<List<string>>(json) is List<string> loadedRecentFiles)
                {
                    CachedRecentFiles.AddRange(loadedRecentFiles);
                }

                IsCached = true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] Failed to cache recent files: {ex.Message}");
            }
        }
    }
}
