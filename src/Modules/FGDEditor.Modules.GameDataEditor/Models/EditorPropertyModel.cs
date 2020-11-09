using Prism.Mvvm;
using System;
using System.Collections.Generic;
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

        public ObjectListModel<EditorPropertyParameterModel> Parameters { get; }

        public string FullDeclaration => $"{Name}({string.Join(", ", Parameters.List.Select(p => p.FullDeclaration))})";

        public EditorPropertyModel(string name, IEnumerable<EditorPropertyParameterModel> parameters)
        {
            _name = name;
            Parameters = new ObjectListModel<EditorPropertyParameterModel>(
                                parameters,
                                () => new EditorPropertyParameterModel(string.Empty, false));

            Parameters.DataChanged += Parameters_DataChanged;
        }

        private void Parameters_DataChanged(object? sender, EventArgs e)
        {
            NotifyFullDeclarationChanged();
        }

        private void NotifyFullDeclarationChanged()
        {
            RaisePropertyChanged(nameof(FullDeclaration));
        }
    }
}
