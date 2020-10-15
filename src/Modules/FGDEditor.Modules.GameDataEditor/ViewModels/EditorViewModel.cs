﻿using FGD.AST;
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

            _gameDataEditor.SyntaxTreeChanged += GameDataEditor_SyntaxTreeChanged;

            _eventAggregator.GetEvent<SaveChangesEvent>().Subscribe(OnSaveChanges);
        }

        public void Destroy()
        {
            _gameDataEditor.SyntaxTreeChanged -= GameDataEditor_SyntaxTreeChanged;
        }

        private void GameDataEditor_SyntaxTreeChanged(object? sender, SyntaxTreeChangedEventArgs e)
        {
            //If we're saving a new tree then the current set of classes is already up-to-date
            if (_savingTree)
            {
                return;
            }

            EntityClasses = null;

            if (!(e.Current is null))
            {
                var list = new ObservableCollection<EntityClassModel>(e.Current.Declarations
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
            }
        }

        private void EntityClasses_CurrentChanged(object? sender, ObjectListCurrentChangedEventArgs<EntityClassModel> e)
        {
            _eventAggregator.GetEvent<CurrentEntityClassChangedEvent>().Publish(e.Current);
        }

        private void OnSaveChanges()
        {
            //Create a new tree that has the new set of classes

            //TODO: First determine the order of the classes

            var declarations = EntityClasses!.List.Select(entityClass => new EntityClass(entityClass.Type, entityClass.Name, entityClass.Description,
                    entityClass.EditorProperties.Select(e => new EditorProperty(
                        e.Name, e.Parameters.Select(p => new EditorPropertyParameter(p.Value, p.IsQuoted)))),
                    entityClass.KeyValues.Select(kv => new KeyValueMapProperty(
                        kv.Name, kv.Type, kv.Description, kv.DefaultValue,
                        kv.Choices.Select(c => new KeyValueChoice(c.Value, c.Description, c.DefaultValue))))
                    ));

            var syntaxTree = new SyntaxTree(declarations);

            _savingTree = true;

            _gameDataEditor.SyntaxTree = syntaxTree;

            _savingTree = false;
        }
    }
}