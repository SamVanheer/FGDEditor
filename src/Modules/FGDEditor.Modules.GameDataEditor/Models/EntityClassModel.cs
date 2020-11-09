using FGD.AST;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class EntityClassModel : BindableBase, IChangeTracking
    {
        private EntityClassType _type;

        public EntityClassType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private string _name;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private string _description;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public ObjectListModel<EditorPropertyModel> EditorProperties { get; }

        public ObjectListModel<KeyValueMapPropertyModel> KeyValues { get; }

        private bool _isChanged;

        public bool IsChanged
        {
            get => _isChanged;
            set => SetProperty(ref _isChanged, value);
        }

        public EntityClassModel(EntityClassType type, string name, string description,
            IEnumerable<EditorPropertyModel> editorProperties, IEnumerable<KeyValueMapPropertyModel> keyValues)
        {
            _type = type;
            _name = name;
            _description = description;

            EditorProperties = new ObjectListModel<EditorPropertyModel>(
                            editorProperties,
                            () => new EditorPropertyModel("NewProperty", Array.Empty<EditorPropertyParameterModel>()));

            //TODO: default values should be defined somewhere else
            KeyValues = new ObjectListModel<KeyValueMapPropertyModel>(
                            keyValues,
                            () => new KeyValueMapPropertyModel("NewKeyValue", "string", string.Empty, string.Empty, Array.Empty<KeyValueChoiceModel>()));

            //These are handled here so ObjectListModel can flag itself as changed
            EditorProperties.DataChanged += EditorProperties_DataChanged;
            KeyValues.DataChanged += KeyValues_DataChanged;
        }

        private void EditorProperties_DataChanged(object? sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(EditorProperties));
        }

        private void KeyValues_DataChanged(object? sender, EventArgs e)
        {
            RaisePropertyChanged(nameof(KeyValues));
        }

        public void AcceptChanges()
        {
            IsChanged = false;

            EditorProperties.AcceptChanges();
            KeyValues.AcceptChanges();
        }
    }
}
