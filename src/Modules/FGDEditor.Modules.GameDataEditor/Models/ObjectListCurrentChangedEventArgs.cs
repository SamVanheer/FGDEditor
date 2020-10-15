using System;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class ObjectListCurrentChangedEventArgs<TObject> : EventArgs
        where TObject : class
    {
        public TObject? Previous { get; }

        public TObject? Current { get; }

        public ObjectListCurrentChangedEventArgs(TObject? previous, TObject? current)
        {
            Previous = previous;
            Current = current;
        }
    }
}
