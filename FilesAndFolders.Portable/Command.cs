﻿using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace FilesAndFolders.Portable
{
    public class Command : ICommand
    {
        private readonly Func<object?, bool>? _canExecute;
        private readonly Action<object?> _execute;
        private readonly WeakEventManager _weakEventManager = new WeakEventManager();

        public Command(Action<object?> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public Command(Action execute) : this(o => execute())
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
        }

        public Command(Action<object?> execute, Func<object?, bool> canExecute) : this(execute)
        {
            _canExecute = canExecute ?? throw new ArgumentNullException(nameof(canExecute));
        }

        public Command(Action execute, Func<bool> canExecute) : this(o => execute(), o => canExecute())
        {
            if (execute == null)
                throw new ArgumentNullException(nameof(execute));
            if (canExecute == null)
                throw new ArgumentNullException(nameof(canExecute));
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public event EventHandler? CanExecuteChanged
        {
            add { _weakEventManager.AddEventHandler(value); }
            remove { _weakEventManager.RemoveEventHandler(value); }
        }

        public void Execute(object? parameter)
        {
            _execute(parameter);
        }

        public void ChangeCanExecute()
        {
            _weakEventManager.HandleEvent(this, EventArgs.Empty, nameof(CanExecuteChanged));
        }
    }

    public class WeakEventManager
    {
        private List<WeakReference<EventHandler?>> _eventHandlers = new List<WeakReference<EventHandler?>>();

        public void AddEventHandler(EventHandler? handler)
        {
            if (handler != null)
            {
                _eventHandlers.Add(new WeakReference<EventHandler?>(handler));
            }
        }

        public void RemoveEventHandler(EventHandler handler)
        {
            _eventHandlers.RemoveAll(wr =>
            {
                if (wr.TryGetTarget(out EventHandler? target))
                    return target == handler;
                return false;
            });
        }

        public void HandleEvent(object sender, EventArgs e, string eventName)
        {
            _eventHandlers.RemoveAll(wr => !wr.TryGetTarget(out EventHandler? _));
            foreach (var weakReference in _eventHandlers)
            {
                if (weakReference.TryGetTarget(out EventHandler? handler))
                {
                    handler?.Invoke(sender, e);
                }
            }
        }
    }
}
