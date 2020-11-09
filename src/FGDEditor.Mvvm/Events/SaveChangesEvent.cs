using FGDEditor.Business;
using Prism.Events;

namespace FGDEditor.Mvvm.Events
{
    /// <summary>
    /// Published when pending changes made to a document should be saved to the document
    /// </summary>
    public sealed class SaveChangesEvent : PubSubEvent<FGDDocument>
    {
    }
}
