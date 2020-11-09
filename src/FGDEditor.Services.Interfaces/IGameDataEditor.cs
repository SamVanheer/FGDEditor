using FGDEditor.Business;
using System;

namespace FGDEditor.Services.Interfaces
{
    public interface IGameDataEditor
    {
        /// <summary>
        /// The current document, if any
        /// </summary>
        FGDDocument? CurrentDocument { get; set; }

        /// <summary>
        /// Raised when the current document is changed
        /// </summary>
        event EventHandler<CurrentDocumentChangedEventArgs> CurrentDocumentChanged;
    }
}
