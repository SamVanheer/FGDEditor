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

        public InputDialogResult ShowInputDialog(string message, string title, InputDialogButton buttons)
        {
            MessageBoxButton messageBoxButtons = buttons switch
            {
                InputDialogButton.Ok => MessageBoxButton.OK,
                InputDialogButton.OkCancel => MessageBoxButton.OKCancel,
                InputDialogButton.YesNo => MessageBoxButton.YesNo,
                InputDialogButton.YesNoCancel => MessageBoxButton.YesNoCancel,
                _ => throw new ArgumentException("Invalid value", nameof(buttons))
            };

            var result = MessageBox.Show(message, title, messageBoxButtons);

            return result switch
            {
                MessageBoxResult.None => InputDialogResult.None,
                MessageBoxResult.OK => InputDialogResult.Ok,
                MessageBoxResult.Cancel => InputDialogResult.Cancel,
                MessageBoxResult.Yes => InputDialogResult.Yes,
                MessageBoxResult.No => InputDialogResult.No,
                _ => throw new InvalidOperationException("Invalid value returned from MessageBox.Show")
            };
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
