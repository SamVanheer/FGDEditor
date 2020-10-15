using Prism.Mvvm;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class EditableString : BindableBase
    {
        private string _value;

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        public EditableString()
        {
            _value = string.Empty;
        }

        public EditableString(string value)
        {
            _value = value;
        }
    }
}
