using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class KeyValueMapPropertyModel : BindableBase
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

        public ObservableCollection<KeyValueChoiceModel> Choices { get; }

        public string ShortDeclaration => $"{Name}({Type})";

        public KeyValueMapPropertyModel(string name, string type, string description, string defaultValue, IEnumerable<KeyValueChoiceModel> choices)
        {
            _name = name;
            _type = type;
            _description = description;
            _defaultValue = defaultValue;
            Choices = new ObservableCollection<KeyValueChoiceModel>(choices);
        }

        public void NotifyShortDeclarationChanged()
        {
            RaisePropertyChanged(nameof(ShortDeclaration));
        }
    }
}
