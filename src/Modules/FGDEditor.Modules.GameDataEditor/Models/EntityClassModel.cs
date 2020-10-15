using FGD.AST;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class EntityClassModel
    {
        public EntityClassType Type { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public ObservableCollection<EditorPropertyModel> EditorProperties { get; }

        public ObservableCollection<KeyValueMapPropertyModel> KeyValues { get; }

        public EntityClassModel(EntityClassType type, string name, string description,
            IEnumerable<EditorPropertyModel> editorProperties, IEnumerable<KeyValueMapPropertyModel> keyValues)
        {
            Type = type;
            Name = name;
            Description = description;
            EditorProperties = new ObservableCollection<EditorPropertyModel>(editorProperties);
            KeyValues = new ObservableCollection<KeyValueMapPropertyModel>(keyValues);
        }
    }
}
