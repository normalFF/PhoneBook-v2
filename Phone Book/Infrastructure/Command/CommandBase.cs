using System;
using System.Windows.Input;

namespace Phone_Book.Infrastructure.Command
{
	internal class CommandBase : ICommand
	{
		private readonly Func<object, bool> _func;
		private readonly Action<object> _action;

		public event EventHandler CanExecuteChanged
		{
			add => CommandManager.RequerySuggested += value;
			remove => CommandManager.RequerySuggested -= value;
		}

		public CommandBase(Action<object> action, Func<object, bool> func = null)
		{
			_action = action ?? throw new NullReferenceException($"{nameof(action)} is null");
			_func = func;
		}

		public bool CanExecute(object parameter) => _func?.Invoke(parameter) ?? false;

		public void Execute(object parameter) => _action.Invoke(parameter);
	}
}
