using FGD.AST;
using FGDEditor.Modules.GameDataEditor.Events;
using FGDEditor.Modules.GameDataEditor.Models;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FGDEditor.Modules.GameDataEditor.ViewModels
{
    public class EntityClassEditorViewModel : BindableBase
    {
        private readonly IEventAggregator _eventAggregator;

        private EntityClassModel? _currentClass;

        public EntityClassModel? CurrentClass
        {
            get => _currentClass;
            set => SetProperty(ref _currentClass, value);
        }

        public static IEnumerable<EntityClassType> EntityClassTypes => Enum.GetValues(typeof(EntityClassType)).Cast<EntityClassType>();

        public EntityClassEditorViewModel(IEventAggregator eventAggregator)
        {
            _eventAggregator = eventAggregator;

            _eventAggregator.GetEvent<CurrentEntityClassChangedEvent>().Subscribe(OnCurrentEntityClassChanged);
        }

        private void OnCurrentEntityClassChanged(EntityClassModel? currentClass)
        {
            CurrentClass = currentClass;
        }
    }
}
