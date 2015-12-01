using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace AssetManagerAdmin.ViewModels
{
    public class MenuItemViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public List<MenuItemViewModel> MenuItems { get; set; }

        private ICommand _command;

        public ICommand Command
        {
            get
            {
                return _command;
            }
        }

        public Action OnClick
        {
            set
            {
                _command = new CommandViewModel(value);
            }
        }
    }

    public class CommandViewModel : ICommand
    {
        private readonly Action _action;

        public CommandViewModel(Action action)
        {
            _action = action;
        }

        public void Execute(object o)
        {
            _action();
        }

        public bool CanExecute(object o)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { }
            remove { }
        }
    }
}
