using Prism.Mvvm;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class EditorPropertyParameterModel : BindableBase
    {
        private string _value;

        public string Value
        {
            get => _value;
            set => SetProperty(ref _value, value);
        }

        private bool _isQuoted;

        public bool IsQuoted
        {
            get => _isQuoted;
            set => SetProperty(ref _isQuoted, value);
        }

        public string FullDeclaration => IsQuoted ? $"\"{Value}\"" : Value;

        public EditorPropertyParameterModel(string value, bool isQuoted)
        {
            _value = value;
            _isQuoted = isQuoted;
        }
    }
}
