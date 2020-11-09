using FGDEditor.Business;
using FGDEditor.Services.Interfaces;
using System;

namespace FGDEditor.Services
{
    public class GameDataEditor : IGameDataEditor
    {
        private FGDDocument? _currentDocument;

        public FGDDocument? CurrentDocument
        {
            get => _currentDocument;

            set
            {
                if (_currentDocument != value)
                {
                    var previous = _currentDocument;
                    _currentDocument = value;

                    DocumentChanged?.Invoke(this, new DocumentChangedEventArgs(previous, _currentDocument));
                }
            }
        }

        public event EventHandler<DocumentChangedEventArgs>? DocumentChanged;
    }
}
