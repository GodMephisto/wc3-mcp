---
type: vendor-doc
source_repo: "https://github.com/whiteqitiandi/GetObjectsInJ"
source_path: "src/GetObjectsInJ/MainForm.cs"
ref: HEAD
---

using System.Diagnostics;

namespace GetObjectsInJ;

public sealed class MainForm : Form
{
    private readonly SourcePicker _jSource = new("JASS 來源", "war3map.j", "JASS 檔案|*.j|所有檔案|*.*");
    private readonly Dictionary<ObjectKind, SourcePicker> _iniSources = new()
    {
        [ObjectKind.Ability] = new SourcePicker("技能資料", "ability.ini", "INI 檔案|*.ini|所有檔案|*.*"),
        [ObjectKind.Buff] = new SourcePicker("增幅資料", "buff.ini", "INI 檔案|*.ini|所有檔案|*.*"),
        [ObjectKind.Item] = new SourcePicker("物品資料", "item.ini", "INI 檔案|*.ini|所有檔案|*.*"),
        [ObjectKind.Unit] = new SourcePicker("部隊資料", "unit.ini", "INI 檔案|*.ini|所有檔案|*.*")
    };
    private readonly Dictionary<ObjectKind, CheckBox> _kindChecks = new();
    private readonly TextBox _outputPath = new() { Dock = DockStyle.Fill };
    private readonly Button _extractButton = new() { Text = "提取並輸出 CSV", AutoSize = true };
    private readonly Label _status = new() { AutoSize = true, ForeColor = Color.DimGray };

    public MainForm()
    {
        Text = "Warcraft III 物件資料提取工具";
        MinimumSize = new Size(780, 680);
        Size = new Size(900, 780);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Microsoft JhengHei UI", 10F);
        AutoScaleMode = AutoScaleMode.Dpi;

        var content = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            Padding = new Padding(18)
        };
        content.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        content.Controls.Add(new Label
        {
            Text = "掃描 war3map.j 內所有 'abcd' 格式四字碼，依四份 INI 分類並輸出名稱。自訂路徑不限制檔名。",
            AutoSize = true,
            ForeColor = Color.FromArgb(55, 65, 81),
            Margin = new Padding(0, 0, 0, 12)
        });
        content.Controls.Add(_jSource);
        foreach (var picker in _iniSources.Values) content.Controls.Add(picker);
        content.Controls.Add(BuildKindSelector());
        content.Controls.Add(BuildOutputSelector());
        content.Controls.Add(BuildActions());

        var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        scroll.Controls.Add(content);
        Controls.Add(scroll);
        AcceptButton = _extractButton;
    }

    private Control BuildKindSelector()
    {
        var group = new GroupBox
        {
            Text = "要輸出的類型（可單選或複選）",
            Dock = DockStyle.Top,
            Height = 72,
            Padding = new Padding(10, 6, 10, 8)
        };
        var flow = new FlowLayoutPanel { Dock = DockStyle.Fill, WrapContents = false };
        foreach (var kind in Enum.GetValues<ObjectKind>())
        {
            var check = new CheckBox
            {
                Text = ObjectDataExtractor.GetKindName(kind),
                Checked = true,
                AutoSize = true,
                Margin = new Padding(3, 8, 24, 3)
            };
            _kindChecks[kind] = check;
            flow.Controls.Add(check);
        }
        group.Controls.Add(flow);
        return group;
    }

    private Control BuildOutputSelector()
    {
        var group = new GroupBox
        {
            Text = "CSV 輸出位置",
            Dock = DockStyle.Top,
            Height = 72,
            Padding = new Padding(10, 8, 10, 8)
        };
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2 };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        var browse = new Button { Text = "瀏覽…", AutoSize = true };
        browse.Click += (_, _) => BrowseOutput();
        _outputPath.Margin = new Padding(0, 2, 8, 2);
        layout.Controls.Add(_outputPath, 0, 0);
        layout.Controls.Add(browse, 1, 0);
        group.Controls.Add(layout);
        return group;
    }

    private Control BuildActions()
    {
        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 72,
            ColumnCount = 2,
            Margin = new Padding(0, 10, 0, 0)
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        _extractButton.Padding = new Padding(12, 5, 12, 5);
        _extractButton.Click += async (_, _) => await ExtractAsync();
        _status.Margin = new Padding(14, 9, 0, 0);
        layout.Controls.Add(_extractButton, 0, 0);
        layout.Controls.Add(_status, 1, 0);
        return layout;
    }

    private void BrowseOutput()
    {
        using var dialog = new SaveFileDialog
        {
            Filter = "CSV 檔案|*.csv",
            DefaultExt = "csv",
            AddExtension = true,
            FileName = string.IsNullOrWhiteSpace(_outputPath.Text) ? "war3_objects.csv" : Path.GetFileName(_outputPath.Text)
        };
        if (!string.IsNullOrWhiteSpace(_outputPath.Text))
            dialog.InitialDirectory = Path.GetDirectoryName(Path.GetFullPath(_outputPath.Text));
        if (dialog.ShowDialog(this) == DialogResult.OK) _outputPath.Text = dialog.FileName;
    }

    private async Task ExtractAsync()
    {
        if (!ValidateInput(out var selectedKinds)) return;

        try
        {
            SetBusy(true, "正在掃描並比對資料…");
            var jPath = _jSource.SelectedPath;
            var sources = _iniSources.Select(pair => new ObjectSource(pair.Key, pair.Value.SelectedPath)).ToArray();
            var result = await Task.Run(() => ObjectDataExtractor.Extract(jPath, sources, selectedKinds));

            IEnumerable<string>? unmatchedToWrite = null;
            if (result.UnmatchedIds.Count > 0)
            {
                var preview = string.Join("、", result.UnmatchedIds.Take(12));
                if (result.UnmatchedIds.Count > 12) preview += "…";
                var answer = MessageBox.Show(this,
                    $"有 {result.UnmatchedIds.Count} 個四字碼無法在四份 INI 中找到對應資料：\n{preview}\n\n是否也將它們以「未匹配」類型列入 CSV？",
                    "發現未匹配的四字碼", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (answer == DialogResult.Yes) unmatchedToWrite = result.UnmatchedIds;
            }

            ObjectDataExtractor.WriteCsv(_outputPath.Text.Trim(), result.MatchedRows, unmatchedToWrite);
            var selectedSummary = string.Join("、", selectedKinds.Select(kind =>
                $"{ObjectDataExtractor.GetKindName(kind)} {result.MatchedCounts[kind]}"));
            _status.ForeColor = Color.FromArgb(22, 101, 52);
            _status.Text = $"完成：{selectedSummary}，未匹配 {result.UnmatchedIds.Count}";

            var open = MessageBox.Show(this, $"CSV 已輸出：\n{_outputPath.Text.Trim()}\n\n要在檔案總管中顯示嗎？",
                "輸出完成", MessageBoxButtons.YesNo, MessageBoxIcon.Information);
            if (open == DialogResult.Yes)
                Process.Start(new ProcessStartInfo("explorer.exe", $"/select,\"{_outputPath.Text.Trim()}\"") { UseShellExecute = true });
        }
        catch (Exception ex)
        {
            _status.ForeColor = Color.Firebrick;
            _status.Text = "提取失敗";
            MessageBox.Show(this, ex.Message, "處理失敗", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _extractButton.Enabled = true;
            Cursor = Cursors.Default;
        }
    }

    private bool ValidateInput(out HashSet<ObjectKind> selectedKinds)
    {
        selectedKinds = _kindChecks.Where(pair => pair.Value.Checked).Select(pair => pair.Key).ToHashSet();
        if (selectedKinds.Count == 0)
        {
            MessageBox.Show(this, "請至少勾選一種要輸出的資料類型。", "資料類型未選擇", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }
        if (!File.Exists(_jSource.SelectedPath)) return InvalidPath("請選擇有效的 war3map.j 來源檔案。", _jSource);
        foreach (var pair in _iniSources)
            if (!File.Exists(pair.Value.SelectedPath))
                return InvalidPath($"請選擇有效的 {GetIniFileName(pair.Key)} 來源檔案。", pair.Value);

        if (string.IsNullOrWhiteSpace(_outputPath.Text))
        {
            MessageBox.Show(this, "請設定 CSV 輸出位置。", "輸出位置未設定", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _outputPath.Focus();
            return false;
        }
        try
        {
            var directory = Path.GetDirectoryName(Path.GetFullPath(_outputPath.Text.Trim()));
            if (string.IsNullOrEmpty(directory) || !Directory.Exists(directory)) throw new DirectoryNotFoundException();
        }
        catch
        {
            MessageBox.Show(this, "CSV 輸出目錄不存在或路徑無效。", "輸出位置無效", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            _outputPath.Focus();
            return false;
        }
        return true;
    }

    private bool InvalidPath(string message, SourcePicker picker)
    {
        MessageBox.Show(this, message, "來源檔案無效", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        picker.FocusPath();
        return false;
    }

    private void SetBusy(bool busy, string text)
    {
        _extractButton.Enabled = !busy;
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
        _status.ForeColor = Color.DimGray;
        _status.Text = text;
    }

    private static string GetIniFileName(ObjectKind kind) => kind switch
    {
        ObjectKind.Ability => "ability.ini",
        ObjectKind.Buff => "buff.ini",
        ObjectKind.Item => "item.ini",
        ObjectKind.Unit => "unit.ini",
        _ => "INI"
    };
}
