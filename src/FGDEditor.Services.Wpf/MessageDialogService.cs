using FGDEditor.Services.Interfaces;
using Microsoft.Win32;
using System;
using System.Windows;

namespace FGDEditor.Services.Wpf
{
    public sealed class MessageDialogService : IMessageDialogService
    {
        public void ShowMessageDialog(MessageDialogType type, string message, string title)
        {
            var image = type switch
            {
                MessageDialogType.Information => MessageBoxImage.Information,
                MessageDialogType.Warning => MessageBoxImage.Warning,
                MessageDialogType.Error => MessageBoxImage.Error,
                _ => throw new ArgumentException("Message dialog type is invalid", nameof(type))
            };

            MessageBox.Show(message, title, MessageBoxButton.OK, image);
        }

        public string? ShowOpenFileDialog(string filter)
        {
            var dialog = new OpenFileDialog
            {
                Filter = filter
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }

            return null;
        }

        public string? ShowSaveFileDialog(string filter)
        {
            var dialog = new SaveFileDialog
            {
                Filter = filter
            };

            if (dialog.ShowDialog() == true)
            {
                return dialog.FileName;
            }

            return null;
        }
    }
}
