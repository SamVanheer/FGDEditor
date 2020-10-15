using FGD.AST;
using FGDEditor.Services.Interfaces;
using System;

namespace FGDEditor.Services
{
    public class GameDataEditor : IGameDataEditor
    {
        private SyntaxTree? _syntaxTree;

        public SyntaxTree? SyntaxTree
        {
            get => _syntaxTree;

            set
            {
                if (_syntaxTree != value)
                {
                    var previous = _syntaxTree;
                    _syntaxTree = value;

                    SyntaxTreeChanged?.Invoke(this, new SyntaxTreeChangedEventArgs(previous, _syntaxTree));
                }
            }
        }

        public event EventHandler<SyntaxTreeChangedEventArgs>? SyntaxTreeChanged;
    }
}
