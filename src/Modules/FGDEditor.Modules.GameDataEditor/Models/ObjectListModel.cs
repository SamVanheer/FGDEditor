using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace FGDEditor.Modules.GameDataEditor.Models
{
    /// <summary>
    /// Helper class to manage a list of objects with add, remove and movement commands
    /// </summary>
    public class ObjectListModel<TObject> : BindableBase
        where TObject : class
    {
        public ObservableCollection<TObject> List { get; }

        private readonly Func<TObject> _objectFactory;

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
            _addObjectCommand ??= new DelegateCommand(ExecuteObjectCommand);

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

        public ObjectListModel(ObservableCollection<TObject> list, Func<TObject> objectFactory)
        {
            List = list;
            _objectFactory = objectFactory;
        }

        private void ExecuteObjectCommand()
        {
            var newObject = _objectFactory();

            List.Add(newObject);

            Current = newObject;
        }

        private void ExecuteRemoveObjectCommand()
        {
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
        }

        private void ExecuteMoveObjectUpCommand() => MoveObject(false);

        private void ExecuteMoveObjectDownCommand() => MoveObject(true);

        private void MoveObject(bool down)
        {
            var index = List.IndexOf(_current!);

            List.Move(index, index + (down ? 1 : -1));

            RaisePropertyChanged(nameof(CanMoveUp));
            RaisePropertyChanged(nameof(CanMoveDown));
        }
    }
}
