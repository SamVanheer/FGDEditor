using FGD.AST;
using System;

namespace FGDEditor.Services.Interfaces
{
    public interface IGameDataEditor
    {
        /// <summary>
        /// The current syntax tree being edited, if any
        /// </summary>
        SyntaxTree? SyntaxTree { get; set; }

        /// <summary>
        /// Raised when the syntax tree is changed
        /// </summary>
        event EventHandler<SyntaxTreeChangedEventArgs> SyntaxTreeChanged;
    }
}
