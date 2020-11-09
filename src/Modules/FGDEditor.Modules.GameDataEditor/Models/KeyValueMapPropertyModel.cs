using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class KeyValueMapPropertyModel : BindableBase, IChangeTracking
    {
        private string _name;

        public string Name
        {
            get => _name;

            set
            {
                if (SetProperty(ref _name, value))
                {
                    RaisePropertyChanged(nameof(ShortDeclaration));
                }
            }
        }

        private string _type;

        public string Type
        {
            get => _type;

            set
            {
                if (SetProperty(ref _type, value))
                {
                    RaisePropertyChanged(nameof(ShortDeclaration));
                }
            }
        }

        private string _description;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _defaultValue;

        public string DefaultValue
        {
            get => _defaultValue;
            set => SetProperty(ref _defaultValue, value);
        }

        public ObjectListModel<KeyValueChoiceModel> Choices { get; }

        public string ShortDeclaration => $"{Name}({Type})";

        private bool _isChanged;

        public bool IsChanged
        {
            get => _isChanged;
            set => SetProperty(ref _isChanged, value);
        }

        public KeyValueMapPropertyModel(string name, string type, string description, string defaultValue, IEnumerable<KeyValueChoiceModel> choices)
        {
            _name = name;
            _type = type;
            _description = description;
            _defaultValue = defaultValue;

            //TODO: define constants somewhere
            Choices = new ObjectListModel<KeyValueChoiceModel>(choices, () => new KeyValueChoiceModel("0", string.Empty, ""));

            Choices.DataChanged += Choices_DataChanged;
        }

        private void Choices_DataChanged(object? sender, EventArgs e)
        {
            IsChanged = true;
        }

        public void AcceptChanges()
        {
            IsChanged = false;
        }
    }
}
