---
type: vendor-doc
source_repo: "https://github.com/escaco95/vjass-project-manager"
source_path: "Utils/InputBox.xaml.cs"
ref: HEAD
---

﻿using System.Windows;
using System.Windows.Input;

namespace vJassMainJBlueprint.Utils
{
    /// <summary>
    /// InputBox.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class InputBox : Window
    {
        private string? Input { get; set; } = null;

        public static string? Show(DependencyObject dependencyObject, string title = "입력", string message = "값을 입력하세요", string defaultValue = "")
        {
            InputBox inputBox = new(title, message, defaultValue)
            {
                Owner = GetWindow(dependencyObject)
            };
            return inputBox.ShowDialog() == true ? inputBox.Input : null;
        }

        private InputBox(string title, string message, string defaultValue)
        {
            InitializeComponent();

            Title = title;
            messageTextBox.Text = message;
            inputTextBox.Text = defaultValue;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            Input = inputTextBox.Text;
            DialogResult = true;
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Input = null;
            DialogResult = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            inputTextBox.SelectAll();
            inputTextBox.Focus();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                OkButton_Click(this, new RoutedEventArgs());
            }
            else if (e.Key == Key.Escape)
            {
                CancelButton_Click(this, new RoutedEventArgs());
            }
        }
    }
}
