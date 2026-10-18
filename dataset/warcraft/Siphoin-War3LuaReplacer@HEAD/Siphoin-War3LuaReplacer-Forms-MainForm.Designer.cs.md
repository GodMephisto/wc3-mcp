---
type: vendor-doc
source_repo: "https://github.com/Siphoin/War3LuaReplacer"
source_path: "Forms/MainForm.Designer.cs"
ref: HEAD
---

﻿namespace War3LuaReplacer
{
    partial class MainForm
    {
        /// <summary>
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.textBoxW3X = new System.Windows.Forms.TextBox();
            this.textBoxLua = new System.Windows.Forms.TextBox();
            this.replaceButton = new System.Windows.Forms.Button();
            this.browseLuaButton = new System.Windows.Forms.Button();
            this.browseW3XButton = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxWC3Exe = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.buttonSelectEXEWC = new System.Windows.Forms.Button();
            this.buttonStartEXE = new System.Windows.Forms.Button();
            this.messageStartExe = new System.Windows.Forms.Label();
            this.checkBoxSaveaths = new System.Windows.Forms.CheckBox();
            this.timerHideMessageStart = new System.Windows.Forms.Timer(this.components);
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // textBoxW3X
            // 
            this.textBoxW3X.Location = new System.Drawing.Point(640, 126);
            this.textBoxW3X.Name = "textBoxW3X";
            this.textBoxW3X.Size = new System.Drawing.Size(361, 20);
            this.textBoxW3X.TabIndex = 0;
            this.textBoxW3X.TextChanged += new System.EventHandler(this.textBoxW3X_TextChanged);
            // 
            // textBoxLua
            // 
            this.textBoxLua.Location = new System.Drawing.Point(640, 50);
            this.textBoxLua.Name = "textBoxLua";
            this.textBoxLua.Size = new System.Drawing.Size(361, 20);
            this.textBoxLua.TabIndex = 1;
            this.textBoxLua.TextChanged += new System.EventHandler(this.textBoxLua_TextChanged);
            // 
            // replaceButton
            // 
            this.replaceButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.replaceButton.Location = new System.Drawing.Point(640, 237);
            this.replaceButton.Name = "replaceButton";
            this.replaceButton.Size = new System.Drawing.Size(192, 44);
            this.replaceButton.TabIndex = 2;
            this.replaceButton.Text = "Заменить файл";
            this.replaceButton.UseVisualStyleBackColor = true;
            this.replaceButton.Click += new System.EventHandler(this.replaceButton_Click);
            // 
            // browseLuaButton
            // 
            this.browseLuaButton.Location = new System.Drawing.Point(1007, 50);
            this.browseLuaButton.Name = "browseLuaButton";
            this.browseLuaButton.Size = new System.Drawing.Size(40, 20);
            this.browseLuaButton.TabIndex = 3;
            this.browseLuaButton.Text = "...";
            this.browseLuaButton.UseVisualStyleBackColor = true;
            this.browseLuaButton.Click += new System.EventHandler(this.browseLuaButton_Click);
            // 
            // browseW3XButton
            // 
            this.browseW3XButton.Location = new System.Drawing.Point(1007, 125);
            this.browseW3XButton.Name = "browseW3XButton";
            this.browseW3XButton.Size = new System.Drawing.Size(40, 20);
            this.browseW3XButton.TabIndex = 4;
            this.browseW3XButton.Text = "...";
            this.browseW3XButton.UseVisualStyleBackColor = true;
            this.browseW3XButton.Click += new System.EventHandler(this.browseW3XButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(636, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(212, 24);
            this.label1.TabIndex = 5;
            this.label1.Text = "Путь до war3map.lua";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.ForeColor = System.Drawing.Color.White;
            this.label2.Location = new System.Drawing.Point(636, 84);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(255, 24);
            this.label2.TabIndex = 6;
            this.label2.Text = "Путь до w3x | w3m карты";
            // 
            // textBoxWC3Exe
            // 
            this.textBoxWC3Exe.Location = new System.Drawing.Point(640, 202);
            this.textBoxWC3Exe.Name = "textBoxWC3Exe";
            this.textBoxWC3Exe.Size = new System.Drawing.Size(361, 20);
            this.textBoxWC3Exe.TabIndex = 7;
            this.textBoxWC3Exe.TextChanged += new System.EventHandler(this.textBoxWC3Exe_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.ForeColor = System.Drawing.Color.White;
            this.label3.Location = new System.Drawing.Point(636, 159);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(392, 24);
            this.label3.TabIndex = 8;
            this.label3.Text = "Путь до Warcraft III Exe (Warcraft III.exe)";
            // 
            // buttonSelectEXEWC
            // 
            this.buttonSelectEXEWC.Location = new System.Drawing.Point(1007, 201);
            this.buttonSelectEXEWC.Name = "buttonSelectEXEWC";
            this.buttonSelectEXEWC.Size = new System.Drawing.Size(40, 20);
            this.buttonSelectEXEWC.TabIndex = 9;
            this.buttonSelectEXEWC.Text = "...";
            this.buttonSelectEXEWC.UseVisualStyleBackColor = true;
            this.buttonSelectEXEWC.Click += new System.EventHandler(this.buttonSelectEXEWC_Click);
            // 
            // buttonStartEXE
            // 
            this.buttonStartEXE.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.buttonStartEXE.Location = new System.Drawing.Point(838, 237);
            this.buttonStartEXE.Name = "buttonStartEXE";
            this.buttonStartEXE.Size = new System.Drawing.Size(192, 44);
            this.buttonStartEXE.TabIndex = 10;
            this.buttonStartEXE.Text = "Запустить карту";
            this.buttonStartEXE.UseVisualStyleBackColor = true;
            this.buttonStartEXE.Click += new System.EventHandler(this.buttonStartEXE_Click);
            // 
            // messageStartExe
            // 
            this.messageStartExe.AutoSize = true;
            this.messageStartExe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.messageStartExe.ForeColor = System.Drawing.Color.Lime;
            this.messageStartExe.Location = new System.Drawing.Point(758, 300);
            this.messageStartExe.Name = "messageStartExe";
            this.messageStartExe.Size = new System.Drawing.Size(153, 16);
            this.messageStartExe.TabIndex = 11;
            this.messageStartExe.Text = "Игра запускается...";
            // 
            // checkBoxSaveaths
            // 
            this.checkBoxSaveaths.AutoSize = true;
            this.checkBoxSaveaths.ForeColor = System.Drawing.Color.White;
            this.checkBoxSaveaths.Location = new System.Drawing.Point(563, 301);
            this.checkBoxSaveaths.Name = "checkBoxSaveaths";
            this.checkBoxSaveaths.Size = new System.Drawing.Size(189, 17);
            this.checkBoxSaveaths.TabIndex = 12;
            this.checkBoxSaveaths.Text = "Сохранить пути после закрытия";
            this.checkBoxSaveaths.UseVisualStyleBackColor = true;
            // 
            // timerHideMessageStart
            // 
            this.timerHideMessageStart.Interval = 2500;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::War3LuaReplacer.Properties.Resources.images;
            this.pictureBox1.Location = new System.Drawing.Point(12, 15);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(506, 301);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 13;
            this.pictureBox1.TabStop = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(1083, 346);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.checkBoxSaveaths);
            this.Controls.Add(this.messageStartExe);
            this.Controls.Add(this.buttonStartEXE);
            this.Controls.Add(this.buttonSelectEXEWC);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBoxWC3Exe);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.browseW3XButton);
            this.Controls.Add(this.browseLuaButton);
            this.Controls.Add(this.replaceButton);
            this.Controls.Add(this.textBoxLua);
            this.Controls.Add(this.textBoxW3X);
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "War3 Lua Replacer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxW3X;
        private System.Windows.Forms.TextBox textBoxLua;
        private System.Windows.Forms.Button replaceButton;
        private System.Windows.Forms.Button browseLuaButton;
        private System.Windows.Forms.Button browseW3XButton;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxWC3Exe;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button buttonSelectEXEWC;
        private System.Windows.Forms.Button buttonStartEXE;
        private System.Windows.Forms.Label messageStartExe;
        private System.Windows.Forms.CheckBox checkBoxSaveaths;
        private System.Windows.Forms.Timer timerHideMessageStart;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}

