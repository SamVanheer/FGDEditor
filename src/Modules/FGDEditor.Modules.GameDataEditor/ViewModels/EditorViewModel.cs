using FGD.AST;
using FGDEditor.Business;
using FGDEditor.Modules.GameDataEditor.Events;
using FGDEditor.Modules.GameDataEditor.Models;
using FGDEditor.Mvvm.Events;
using FGDEditor.Services.Interfaces;
using Prism.Events;
using Prism.Mvvm;
using Prism.Navigation;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace FGDEditor.Modules.GameDataEditor.ViewModels
{
    public class EditorViewModel : BindableBase, IDestructible
    {
        private readonly IEventAggregator _eventAggregator;
        private readonly IGameDataEditor _gameDataEditor;

        private ObjectListModel<EntityClassModel>? _entityClasses;

        public ObjectListModel<EntityClassModel>? EntityClasses
        {
            get => _entityClasses;
            set => SetProperty(ref _entityClasses, value);
        }

        private bool _savingTree;

        public EditorViewModel(IEventAggregator eventAggregator, IGameDataEditor gameDataEditor)
        {
            _eventAggregator = eventAggregator;
            _gameDataEditor = gameDataEditor;

            _gameDataEditor.CurrentDocumentChanged += GameDataEditor_DocumentChanged;

            _eventAggregator.GetEvent<SaveChangesEvent>().Subscribe(OnSaveChanges);
        }

        public void Destroy()
        {
            _gameDataEditor.CurrentDocumentChanged -= GameDataEditor_DocumentChanged;
        }

        private void GameDataEditor_DocumentChanged(object? sender, CurrentDocumentChangedEventArgs e)
        {
            //If we're saving a new tree then the current set of classes is already up-to-date
            if (_savingTree)
            {
                return;
            }

            if (!(EntityClasses is null))
            {
                EntityClasses.DataChanged -= EntityClasses_DataChanged;
                EntityClasses.CurrentChanged -= EntityClasses_CurrentChanged;
                EntityClasses = null;
            }

            if (!(e.Current is null))
            {
                var list = new ObservableCollection<EntityClassModel>(e.Current.SyntaxTree.Declarations
                    .OfType<EntityClass>()
                    .Select(e => new EntityClassModel(e.Type, e.Name, e.Description,
                        e.EditorProperties.Select(p => new EditorPropertyModel(p.Name,
                            p.Parameters.Select(p => new EditorPropertyParameterModel(p.Value, p.IsQuoted)))),
                        e.MapProperties
                            .OfType<KeyValueMapProperty>()
                            .Select(kv => new KeyValueMapPropertyModel(kv.Name, kv.Type, kv.Description, kv.DefaultValue,
                                kv.Choices.Select(c => new KeyValueChoiceModel(c.Value, c.Description, c.DefaultValue))))))
                );

                EntityClasses = new ObjectListModel<EntityClassModel>(
                    list,
                    () => new EntityClassModel(EntityClassType.BaseClass, "NewEntityClass", string.Empty,
                        Array.Empty<EditorPropertyModel>(), Array.Empty<KeyValueMapPropertyModel>()));

                EntityClasses.CurrentChanged += EntityClasses_CurrentChanged;
                EntityClasses.DataChanged += EntityClasses_DataChanged;
            }
        }

        private void EntityClasses_CurrentChanged(object? sender, ObjectListCurrentChangedEventArgs<EntityClassModel> e)
        {
            _eventAggregator.GetEvent<CurrentEntityClassChangedEvent>().Publish(e.Current);
        }

        private void EntityClasses_DataChanged(object? sender, EventArgs e)
        {
            _gameDataEditor.CurrentDocument!.HasUnsavedChanges = true;
        }

        private void OnSaveChanges(FGDDocument document)
        {
            //Create a new tree that has the new set of classes

            //TODO: First determine the order of the classes

            var declarations = EntityClasses!.List.Select(entityClass => new EntityClass(entityClass.Type, entityClass.Name, entityClass.Description,
                    entityClass.EditorProperties.List.Select(e => new EditorProperty(
                        e.Name, e.Parameters.List.Select(p => new EditorPropertyParameter(p.Value, p.IsQuoted)))),
                    entityClass.KeyValues.List.Select(kv => new KeyValueMapProperty(
                        kv.Name, kv.Type, kv.Description, kv.DefaultValue,
                        kv.Choices.List.Select(c => new KeyValueChoice(c.Value, c.Description, c.DefaultValue))))
                    ));

            var syntaxTree = new SyntaxTree(declarations);

            _savingTree = true;

            document.SyntaxTree = syntaxTree;

            EntityClasses!.AcceptChanges();

            _savingTree = false;
        }
    }
}
