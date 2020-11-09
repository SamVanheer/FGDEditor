using FGD.Grammar;
using FGD.Serialization;
using FGDEditor.Business;
using FGDEditor.Mvvm.Events;
using FGDEditor.Services.Interfaces;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using System.IO;
using System.Windows;

namespace FGDEditor.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IMessageDialogService _messageDialogService;
        private readonly IGameDataEditor _gameDataEditor;

        private string _title = "FGD Editor";

        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private DelegateCommand? _openFileCommand;

        public DelegateCommand OpenFileCommand => _openFileCommand ??= new DelegateCommand(ExecuteOpenFileCommand);

        public bool HasFileOpen => !(_gameDataEditor.CurrentDocument is null);

        private DelegateCommand? _saveFileCommand;

        public DelegateCommand SaveFileCommand => _saveFileCommand ??= new DelegateCommand(ExecuteSaveFileCommand)
            .ObservesCanExecute(() => HasFileOpen);

        private DelegateCommand? _closeCommand;

        public DelegateCommand CloseCommand => _closeCommand ??= new DelegateCommand(ExecuteCloseCommand)
            .ObservesCanExecute(() => HasFileOpen);

        private DelegateCommand? _exitCommand;

        public DelegateCommand ExitCommand => _exitCommand ??= new DelegateCommand(ExecuteExitCommand);

        public MainWindowViewModel(IEventAggregator eventAggregator, IMessageDialogService messageDialogService, IGameDataEditor gameDataEditor)
        {
            _eventAggregator = eventAggregator;
            _messageDialogService = messageDialogService;
            _gameDataEditor = gameDataEditor;
        }

        private void ExecuteOpenFileCommand()
        {
            //TODO: if a file is already open the user should first be prompted to save it

            var fileName = _messageDialogService.ShowOpenFileDialog("FGD Files (.fgd)|*.fgd|All Files|*.*");

            if (!(fileName is null))
            {
                try
                {
                    using var stream = File.OpenText(fileName);

                    var parser = new FGDGrammarBasedParser(GrammarTypes.HalfLife1);

                    _gameDataEditor.CurrentDocument = new FGDDocument(parser.Parse(stream));

                    RaisePropertyChanged(nameof(HasFileOpen));
                }
                catch (FGDParseException e)
                {
                    _messageDialogService.ShowMessageDialog(
                        MessageDialogType.Error, $"An error occurred while parsing the fgd file:\n{e.Message}", "Error parsing fgd file");
                }
                catch (IOException e)
                {
                    _messageDialogService.ShowMessageDialog(
                        MessageDialogType.Error, $"An IO error occurred while parsing the fgd file:\n{e.Message}", "Error parsing fgd file");
                }
            }
        }

        private void ExecuteSaveFileCommand()
        {
            var fileName = _messageDialogService.ShowSaveFileDialog("FGD Files (.fgd)|*.fgd|All Files|*.*");

            if (!(fileName is null))
            {
                //TODO: this event should pass the document being saved
                _eventAggregator.GetEvent<SaveChangesEvent>().Publish();

                try
                {
                    using var stream = File.CreateText(fileName);

                    var writer = new FGDWriter();

                    writer.Write(_gameDataEditor.CurrentDocument!.SyntaxTree, stream);
                }
                catch (IOException e)
                {
                    _messageDialogService.ShowMessageDialog(
                        MessageDialogType.Error, $"An IO error occurred while writing the fgd file:\n{e.Message}", "Error writing fgd file");
                }
            }
        }

        private void ExecuteCloseCommand()
        {
            //TODO: prompt to save

            _gameDataEditor.CurrentDocument = null;
            RaisePropertyChanged(nameof(HasFileOpen));
        }

        private void ExecuteExitCommand()
        {
            //TODO: if there are pending changes ask to save before shutting down

            Application.Current.Shutdown();
        }
    }
}
