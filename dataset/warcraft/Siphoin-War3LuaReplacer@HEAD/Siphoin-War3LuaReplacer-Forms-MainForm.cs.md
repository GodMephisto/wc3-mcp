---
type: vendor-doc
source_repo: "https://github.com/Siphoin/War3LuaReplacer"
source_path: "Forms/MainForm.cs"
ref: HEAD
---

﻿using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using War3LuaReplacer.Models;

namespace War3LuaReplacer
{
    public partial class MainForm : Form
    {
        private const string GRAPHICS_API = "Direct3D9";
        private const bool PAUSE_GAME_ON_LOSE_FOCUS = false;

        private readonly string PATH_SAVE_FILE = Path.Combine(Environment.CurrentDirectory, "save/data.json");
        private readonly string PATH_SAVE_DIRECTORY   = Path.Combine(Environment.CurrentDirectory, "save/");
        public MainForm()
        {
            InitializeComponent();
            LoadUserData();
            CheckTextBoxs();
            messageStartExe.Visible = false;
        }


        private void replaceButton_Click(object sender, EventArgs e)
        {
            string w3xPath = textBoxW3X.Text;
            string luaPath = textBoxLua.Text;
            string tempDir = null;

            try
            {
                // Проверка существования файлов
                if (!File.Exists(w3xPath) || !File.Exists(luaPath))
                {
                    MessageBox.Show("Файлы не найдены!", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Создание временной директории
                tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
                Directory.CreateDirectory(tempDir);

                // Полные пути к файлам
                string mpqEditorPath = Path.Combine(Application.StartupPath, "MPQEditor.exe");
                string tempW3x = Path.Combine(tempDir, "temp.w3x");
                string tempLua = Path.Combine(tempDir, "war3map.lua");

                // Копирование исходного архива
                File.Copy(w3xPath, tempW3x);

                // Удаляем старый файл из архива
                ExecuteMpqEditor(mpqEditorPath, $"delete \"{tempW3x}\" war3map.lua");

                // Копируем новый Lua-файл во временную папку
                File.Copy(luaPath, tempLua);

                // Добавляем файл В КОРЕНЬ архива (третий аргумент - путь внутри архива)
                ExecuteMpqEditor(mpqEditorPath, $"add \"{tempW3x}\" \"{tempLua}\" war3map.lua");

                // Заменяем оригинальный файл
                File.Copy(tempW3x, w3xPath, true);

                MessageBox.Show("Файл успешно заменён!", "Успех",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                              MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                // Удаление временной директории
                if (tempDir != null && Directory.Exists(tempDir))
                {
                    try
                    {
                        Directory.Delete(tempDir, true);
                    }
                    catch { /* Игнорируем ошибки удаления */ }
                }
            }
        }

        private void browseW3XButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Warcraft 3 Map Files (*.w3x, *.w3m)|*.w3x;*.w3m|All Files (*.*)|*.*";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxW3X.Text = openFileDialog.FileName;
                }
            }
        }

        private void browseLuaButton_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Lua Files (*.lua)|*.lua";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxLua.Text = openFileDialog.FileName;
                }
            }
        }

        private void ExecuteMpqEditor(string editorPath, string arguments)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = editorPath,
                    Arguments = arguments,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                }
            };

            process.Start();
            process.WaitForExit();

            if (process.ExitCode != 0)
            {
                string error = process.StandardError.ReadToEnd();
                throw new Exception($"MPQEditor error: {error}");
            }
        }

        public void TestMap(string mapPath)
        {
            string wc3exe = textBoxWC3Exe.Text;

            if (!File.Exists(wc3exe))
            {
                throw new FileNotFoundException("не найден исполняемый файл Warcraft III.");
            }

            if (!File.Exists(mapPath))
            {
                throw new FileNotFoundException("не найден файл карты, такого пути к к карте не существует.");
            }

            var commandLineArgs = new StringBuilder();
            var isReforged = Version.Parse(FileVersionInfo.GetVersionInfo(wc3exe).FileVersion) >= new Version(1, 32);
            if (isReforged)
            {
                commandLineArgs.Append(" -launch");
            }
            else if (GRAPHICS_API != null)
            {
                commandLineArgs.Append($" -graphicsapi {GRAPHICS_API}");
            }

            if (!PAUSE_GAME_ON_LOSE_FOCUS)
            {
                commandLineArgs.Append(" -nowfpause");
            }

            var absoluteMapPath = new FileInfo(mapPath).FullName;
            commandLineArgs.Append($" -loadfile \"{absoluteMapPath}\"");

            Process.Start(wc3exe, commandLineArgs.ToString());
        }

        private void CheckTextBoxs ()
        {
            buttonStartEXE.Enabled = !string.IsNullOrEmpty(textBoxW3X.Text) && !string.IsNullOrEmpty (textBoxWC3Exe.Text);
            replaceButton.Enabled = !string.IsNullOrEmpty(textBoxLua.Text) && !string.IsNullOrEmpty(textBoxW3X.Text);
        }

        private void SetStateStaring()
        {
            messageStartExe.Visible = true;
            buttonStartEXE.Enabled = false;
            timerHideMessageStart.Tick += OnTickTimer;
            timerHideMessageStart.Start();
        }

        private void OnTickTimer(object sender, EventArgs e)
        {
            timerHideMessageStart.Tick -= OnTickTimer;
            timerHideMessageStart.Stop();
            buttonStartEXE.Enabled = true;
            messageStartExe.Visible = false;
        }

        private void buttonStartEXE_Click(object sender, EventArgs e)
        {
            try
            {
                TestMap(textBoxW3X.Text);
                SetStateStaring();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
              MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        private void textBoxWC3Exe_TextChanged(object sender, EventArgs e)
        {
           CheckTextBoxs();
        }

        private void textBoxW3X_TextChanged(object sender, EventArgs e)
        {
            CheckTextBoxs();
        }

        private void textBoxLua_TextChanged(object sender, EventArgs e)
        {
            CheckTextBoxs();
        }

        private void buttonSelectEXEWC_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "Warcraft III EXE (*.exe)|*.exe";
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    textBoxWC3Exe.Text = openFileDialog.FileName;
                }
            }
        }

        #region Save & Load User Data
        private void LoadUserData ()
        {
            if (File.Exists(PATH_SAVE_FILE))
            {
                try
                {
                    string json = File.ReadAllText(PATH_SAVE_FILE);
                    SavedUserData savedUserData = JsonConvert.DeserializeObject<SavedUserData>(json);

                    if (!savedUserData.SaveAfterExit)
                    {
                        File.Delete(PATH_SAVE_FILE);
                        return;
                    }
                    textBoxLua.Text = savedUserData.LuaPath;
                    textBoxW3X.Text = savedUserData.MapPath;
                    textBoxWC3Exe.Text = savedUserData.WC3ExePath;
                    checkBoxSaveaths.Checked = savedUserData.SaveAfterExit;
                }
                catch (Exception ex)
                {

                    MessageBox.Show($"Ошибка: {ex.Message}", "Ошибка",
                                  MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (checkBoxSaveaths.Checked)
            {
                if (!Directory.Exists(PATH_SAVE_DIRECTORY))
                {
                    Directory.CreateDirectory(PATH_SAVE_DIRECTORY);
                }

                SavedUserData savedUserData = new SavedUserData();
                savedUserData.SaveAfterExit = true;
                savedUserData.LuaPath = textBoxLua.Text;
                savedUserData.WC3ExePath = textBoxWC3Exe.Text;
                savedUserData.MapPath = textBoxW3X.Text;

                string json = JsonConvert.SerializeObject(savedUserData);

                File.WriteAllText(PATH_SAVE_FILE, json);
            }

            else
            {
                if (File.Exists(PATH_SAVE_FILE))
                {
                    File.Delete(PATH_SAVE_FILE);
                }
            }
        }
    }
}