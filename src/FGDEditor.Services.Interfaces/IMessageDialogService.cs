namespace FGDEditor.Services.Interfaces
{
    public interface IMessageDialogService
    {
        void ShowMessageDialog(MessageDialogType type, string message, string title);

        InputDialogResult ShowInputDialog(string message, string title, InputDialogButton buttons);

        string? ShowOpenFileDialog(string filter);

        string? ShowSaveFileDialog(string filter);
    }
}
