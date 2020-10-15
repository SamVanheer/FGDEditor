namespace FGDEditor.Services.Interfaces
{
    public interface IMessageDialogService
    {
        void ShowMessageDialog(MessageDialogType type, string message, string title);

        string? ShowOpenFileDialog(string filter);

        string? ShowSaveFileDialog(string filter);
    }
}
