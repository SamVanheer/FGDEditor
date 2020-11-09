using Prism.Mvvm;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class KeyValueChoiceModel : BindableBase
    {
        private string _value;

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
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

        public KeyValueChoiceModel(string value, string description, string defaultValue)
        {
            _value = value;
            _description = description;
            _defaultValue = defaultValue;
        }
    }
}
