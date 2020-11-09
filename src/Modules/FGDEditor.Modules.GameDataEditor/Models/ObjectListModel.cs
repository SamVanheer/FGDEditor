using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    /// <summary>
    /// Helper class to manage a list of objects with add, remove and movement commands
    /// Tracks whether changes have been made to it
    /// </summary>
    public class ObjectListModel<TObject> : BindableBase, IChangeTracking
        where TObject : class
    {
        public ObservableCollection<TObject> List { get; }

        private readonly Func<TObject> _objectFactory;

        private bool _ignoreObjectChanges;

        private TObject? _current;

        public TObject? Current
        {
            get => _current;

            set
            {
                var previous = _current;

                if (SetProperty(ref _current, value))
                {
                    CurrentChanged?.Invoke(this, new ObjectListCurrentChangedEventArgs<TObject>(previous, _current));
                }
            }
        }

        public bool HasSelectedObject => !(_current is null);

        public bool CanMoveUp => HasSelectedObject
            && (List.IndexOf(_current!) > 0);

        public bool CanMoveDown => HasSelectedObject
            && (List.IndexOf(_current!) < (List.Count - 1));

        private DelegateCommand? _addObjectCommand;

        public DelegateCommand AddObjectCommand =>
            _addObjectCommand ??= new DelegateCommand(ExecuteAddObjectCommand);

        private DelegateCommand? _removeObjectCommand;

        public DelegateCommand RemoveObjectCommand =>
            _removeObjectCommand ??= new DelegateCommand(ExecuteRemoveObjectCommand)
                .ObservesCanExecute(() => HasSelectedObject)
                .ObservesProperty(() => Current);

        private DelegateCommand? _moveObjectUpCommand;

        public DelegateCommand MoveObjectUpCommand =>
            _moveObjectUpCommand ??= new DelegateCommand(ExecuteMoveObjectUpCommand)
                .ObservesCanExecute(() => CanMoveUp)
                .ObservesProperty(() => Current);

        private DelegateCommand? _moveObjectDownCommand;

        public DelegateCommand MoveObjectDownCommand =>
            _moveObjectDownCommand ??= new DelegateCommand(ExecuteMoveObjectDownCommand)
                .ObservesCanExecute(() => CanMoveDown)
                .ObservesProperty(() => Current);

        public event EventHandler<ObjectListCurrentChangedEventArgs<TObject>>? CurrentChanged;

        public bool IsChanged { get; private set; }

        /// <summary>
        /// Raised when any data changes, such as the order of list elements
        /// </summary>
        public event EventHandler? DataChanged;

        public ObjectListModel(IEnumerable<TObject> list, Func<TObject> objectFactory)
        {
            List = new ObservableCollection<TObject>(list);
            _objectFactory = objectFactory;

            if (typeof(INotifyPropertyChanged).IsAssignableFrom(typeof(TObject)))
            {
                foreach (var propertyChanged in List.Cast<INotifyPropertyChanged>())
                {
                    propertyChanged.PropertyChanged += TObject_PropertyChanged;
                }
            }
        }

        private void ExecuteAddObjectCommand()
        {
            var newObject = _objectFactory();

            List.Add(newObject);

            Current = newObject;

            OnDataChanged();

            //Listen for changes so they can be cascaded upwards
            //TODO: maybe listen to the list directly to catch changes made externally
            if (newObject is INotifyPropertyChanged propertyChanged)
            {
                propertyChanged.PropertyChanged += TObject_PropertyChanged;
            }
        }

        private void ExecuteRemoveObjectCommand()
        {
            if (_current is INotifyPropertyChanged propertyChanged)
            {
                propertyChanged.PropertyChanged -= TObject_PropertyChanged;
            }

            var index = List.IndexOf(_current!);

            List.RemoveAt(index);

            //Try to select the object that is now in the same spot as the removed one
            if (index < List.Count)
            {
                Current = List[index];
            }
            //Try to select the object that was right before the removed one
            else if (index > 0 && ((index - 1) < List.Count))
            {
                Current = List[index - 1];
            }
            else
            {
                //Explicitly set to null so listeners are aware
                Current = null;
                RaisePropertyChanged(nameof(Current));
            }

            OnDataChanged();
        }

        private void ExecuteMoveObjectUpCommand() => MoveObject(false);

        private void ExecuteMoveObjectDownCommand() => MoveObject(true);

        private void MoveObject(bool down)
        {
            var index = List.IndexOf(_current!);

            List.Move(index, index + (down ? 1 : -1));

            RaisePropertyChanged(nameof(CanMoveUp));
            RaisePropertyChanged(nameof(CanMoveDown));

            OnDataChanged();
        }

        private void TObject_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_ignoreObjectChanges)
            {
                return;
            }

            OnDataChanged();
        }

        private void OnDataChanged()
        {
            IsChanged = true;
            DataChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AcceptChanges()
        {
            IsChanged = false;

            //If the object type tracks changes cascade the change acceptance downward
            if (typeof(IChangeTracking).IsAssignableFrom(typeof(TObject)))
            {
                _ignoreObjectChanges = true;

                foreach (var tracking in List.Cast<IChangeTracking>())
                {
                    tracking.AcceptChanges();
                }

                _ignoreObjectChanges = false;
            }
        }
    }
}
