---
type: vendor-doc
source_repo: "https://github.com/whiteqitiandi/GetObjectsInJ"
source_path: "src/GetObjectsInJ/SourcePicker.cs"
ref: HEAD
---

namespace GetObjectsInJ;

internal sealed class SourcePicker : GroupBox
{
    private readonly RadioButton _rootRadio = new() { AutoSize = true };
    private readonly RadioButton _customRadio = new() { Text = "自訂路徑", AutoSize = true };
    private readonly TextBox _customPath = new() { Dock = DockStyle.Fill };
    private readonly Button _browseButton = new() { Text = "瀏覽…", AutoSize = true };
    private readonly string _rootPath;
    private readonly string _filter;

    public SourcePicker(string title, string rootFileName, string filter)
    {
        Text = title;
        Dock = DockStyle.Top;
        Height = 96;
        Padding = new Padding(10, 6, 10, 8);

        _rootPath = Path.Combine(AppContext.BaseDirectory, rootFileName);
        _filter = filter;
        _rootRadio.Text = $"程式根目錄：{rootFileName}";
        _rootRadio.Enabled = File.Exists(_rootPath);
        _rootRadio.Checked = _rootRadio.Enabled;
        _customRadio.Checked = !_rootRadio.Enabled;

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 2,
            Margin = Padding.Empty
        };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 50));

        layout.Controls.Add(_rootRadio, 0, 0);
        layout.SetColumnSpan(_rootRadio, 3);
        layout.Controls.Add(_customRadio, 0, 1);
        layout.Controls.Add(_customPath, 1, 1);
        layout.Controls.Add(_browseButton, 2, 1);

        _customPath.Margin = new Padding(8, 2, 8, 2);
        _browseButton.Margin = new Padding(0);
        _rootRadio.CheckedChanged += (_, _) => UpdateCustomControls();
        _customRadio.CheckedChanged += (_, _) => UpdateCustomControls();
        _browseButton.Click += (_, _) => Browse();
        Controls.Add(layout);
        UpdateCustomControls();
    }

    public string SelectedPath => _rootRadio.Checked ? _rootPath : _customPath.Text.Trim();

    public void FocusPath()
    {
        if (!_customRadio.Checked) _customRadio.Checked = true;
        _customPath.Focus();
    }

    private void UpdateCustomControls()
    {
        _customPath.Enabled = _customRadio.Checked;
        _browseButton.Enabled = _customRadio.Checked;
    }

    private void Browse()
    {
        using var dialog = new OpenFileDialog
        {
            Filter = _filter,
            CheckFileExists = true,
            Title = $"選擇{Text}"
        };
        if (File.Exists(_customPath.Text)) dialog.InitialDirectory = Path.GetDirectoryName(_customPath.Text);
        if (dialog.ShowDialog(this) == DialogResult.OK) _customPath.Text = dialog.FileName;
    }
}
