using FGD.AST;
using FGDEditor.Modules.GameDataEditor.Events;
using FGDEditor.Modules.GameDataEditor.Models;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
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

            set
            {
                if (SetProperty(ref _currentClass, value))
                {
                    if (!(_currentClass is null))
                    {
                        EditorPropertyList = new ObjectListModel<EditorPropertyModel>(
                            _currentClass.EditorProperties,
                            () => new EditorPropertyModel("NewProperty", Array.Empty<EditorPropertyParameterModel>()));

                        //TODO: default values should be defined somewhere else
                        KeyValueList = new ObjectListModel<KeyValueMapPropertyModel>(
                            _currentClass.KeyValues,
                            () => new KeyValueMapPropertyModel("NewKeyValue", "string", string.Empty, string.Empty, Array.Empty<KeyValueChoiceModel>()));
                    }
                    else
                    {
                        EditorPropertyList = null;
                        KeyValueList = null;
                    }
                }
            }
        }

        #region Editor Property Properties

        private ObjectListModel<EditorPropertyModel>? _editorPropertyList;

        public ObjectListModel<EditorPropertyModel>? EditorPropertyList
        {
            get => _editorPropertyList;

            set
            {
                if (SetProperty(ref _editorPropertyList, value))
                {
                    if (!(_editorPropertyList is null))
                    {
                        _editorPropertyList.CurrentChanged += EditorPropertyList_CurrentChanged;
                    }

                    //New lists always initialize Current to null so there is nothing to start with
                    EditorPropertyParameterList = null;
                }
            }
        }

        private ObjectListModel<EditorPropertyParameterModel>? _editorPropertyParameterList;

        public ObjectListModel<EditorPropertyParameterModel>? EditorPropertyParameterList
        {
            get => _editorPropertyParameterList;

            set
            {
                if (SetProperty(ref _editorPropertyParameterList, value))
                {
                    if (!(_editorPropertyParameterList is null))
                    {
                        _editorPropertyParameterList.CurrentChanged += EditorPropertyParameterList_CurrentChanged;
                    }
                }
            }
        }

        #endregion

        #region KeyValue Properties

        private ObjectListModel<KeyValueMapPropertyModel>? _keyValueList;

        public ObjectListModel<KeyValueMapPropertyModel>? KeyValueList
        {
            get => _keyValueList;

            set
            {
                if (SetProperty(ref _keyValueList, value) && !(_keyValueList is null))
                {
                    _keyValueList.CurrentChanged += KeyValueList_CurrentChanged;
                }
            }
        }

        private ObjectListModel<KeyValueChoiceModel>? _keyValueChoiceList;

        public ObjectListModel<KeyValueChoiceModel>? KeyValueChoiceList
        {
            get => _keyValueChoiceList;
            set => SetProperty(ref _keyValueChoiceList, value);
        }

        #endregion

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

        #region Editor Property Methods

        private void EditorPropertyList_CurrentChanged(object? sender, ObjectListCurrentChangedEventArgs<EditorPropertyModel> e)
        {
            if (!(e.Previous is null))
            {
                e.Previous.Parameters.CollectionChanged -= CurrentEditorPropertyParameters_CollectionChanged;
            }

            if (!(e.Current is null))
            {
                e.Current.Parameters.CollectionChanged += CurrentEditorPropertyParameters_CollectionChanged;

                EditorPropertyParameterList = new ObjectListModel<EditorPropertyParameterModel>(
                                e.Current.Parameters,
                                () => new EditorPropertyParameterModel(string.Empty, false));
            }
            else
            {
                EditorPropertyParameterList = null;
            }
        }

        private void CurrentEditorPropertyParameters_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _editorPropertyList!.Current!.NotifyFullDeclarationChanged();
        }

        private void EditorPropertyParameterList_CurrentChanged(object? sender, ObjectListCurrentChangedEventArgs<EditorPropertyParameterModel> e)
        {
            if (!(e.Previous is null))
            {
                e.Previous.PropertyChanged -= CurrentEditorPropertyParameter_PropertyChanged;
            }

            if (!(e.Current is null))
            {
                e.Current.PropertyChanged += CurrentEditorPropertyParameter_PropertyChanged;
            }
        }

        private void CurrentEditorPropertyParameter_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _editorPropertyList!.Current!.NotifyFullDeclarationChanged();
        }

        #endregion

        #region KeyValue Methods

        private void KeyValueList_CurrentChanged(object? sender, ObjectListCurrentChangedEventArgs<KeyValueMapPropertyModel> e)
        {
            if (!(e.Current is null))
            {
                KeyValueChoiceList = new ObjectListModel<KeyValueChoiceModel>(e.Current.Choices, () => new KeyValueChoiceModel("0", string.Empty, ""));
            }
            else
            {
                KeyValueChoiceList = null;
            }
        }

        #endregion
    }
}
