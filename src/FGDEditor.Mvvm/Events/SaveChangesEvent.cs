using Prism.Events;

namespace FGDEditor.Mvvm.Events
{
    /// <summary>
    /// Published when pending changes made to a syntax tree should be saved to the tree
    /// </summary>
    public sealed class SaveChangesEvent : PubSubEvent
    {
    }
}
