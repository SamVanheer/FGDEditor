using Prism.Mvvm;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    public sealed class EditorPropertyModel : BindableBase
    {
        private string _name;

        public string Name
        {
            get => _name;

            set
            {
                if (SetProperty(ref _name, value))
                {
                    NotifyFullDeclarationChanged();
                }
            }
        }

        public ObservableCollection<EditorPropertyParameterModel> Parameters { get; }

        public string FullDeclaration => $"{Name}({string.Join(", ", Parameters.Select(p => p.FullDeclaration))})";

        public EditorPropertyModel(string name, IEnumerable<EditorPropertyParameterModel> parameters)
        {
            _name = name;
            Parameters = new ObservableCollection<EditorPropertyParameterModel>(parameters);
        }

        public void NotifyFullDeclarationChanged()
        {
            RaisePropertyChanged(nameof(FullDeclaration));
        }
    }
}
