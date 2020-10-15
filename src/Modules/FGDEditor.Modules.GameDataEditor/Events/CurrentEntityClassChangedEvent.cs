using FGDEditor.Modules.GameDataEditor.Models;
using Prism.Events;

namespace FGDEditor.Modules.GameDataEditor.Events
{
    public sealed class CurrentEntityClassChangedEvent : PubSubEvent<EntityClassModel?>
    {
    }
}
