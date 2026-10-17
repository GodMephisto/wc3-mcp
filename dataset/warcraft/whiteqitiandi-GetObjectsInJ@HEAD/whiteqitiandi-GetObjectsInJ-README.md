---
type: vendor-doc
source_repo: "https://github.com/whiteqitiandi/GetObjectsInJ"
source_path: "README.md"
ref: HEAD
---

# Warcraft III 物件資料提取工具

Windows 桌面小工具，掃描 `war3map.j` 中所有由英文字母或數字組成的四字碼（例如 `'A001'`、`'B002'`、`'hdhw'`），再使用以下資料檔分類並取得 `Name`：

- `ability.ini`：技能
- `buff.ini`：增幅
- `item.ini`：物品
- `unit.ini`：部隊

每一個來源皆可用 RadioButton 選擇「程式根目錄固定檔名」或「自訂路徑」。根目錄沒有對應檔案時，該選項會自動停用；自訂路徑不限制檔名。輸出類型可用 CheckBox 單選或複選，無法在四份 INI 中匹配的四字碼會在輸出前詢問是否列入。

INI 解析支援 UTF-8、UTF-16、Big5，以及 Lua 長字串 `[[...]]`、`[=[...]=]` 等任意等號層級。Buff 會使用 `Bufftip`（其次 `EditorName`）作為名稱；即使 section 沒有名稱欄位，仍會依 ID 正確判定為已匹配。CSV 不含標題列，每列依序為 `ID,名稱,類型`，並使用含 BOM 的 UTF-8，方便 Excel 正確辨識繁體中文。

## 建置與測試

需要 Windows 與 .NET 10 SDK：

```powershell
dotnet build GetObjectsInJ.sln
dotnet run --project tests/GetObjectsInJ.Tests
dotnet publish src/GetObjectsInJ/GetObjectsInJ.csproj -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true -o release
```
