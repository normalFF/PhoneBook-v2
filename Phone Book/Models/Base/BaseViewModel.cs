﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using PhoneLibrary;

namespace Phone_Book.Models.Base
{
	internal abstract class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;

		public bool Set<T>(ref T currentValue, T newValue, [CallerMemberName] string parameter = null)
		{
			if (Equals(currentValue, newValue)) return false;
			currentValue = newValue;
			OnPropertyChanged(parameter);
			return true;
		}

		public void OnPropertyChanged([CallerMemberName] string parameter = null)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(parameter));
		}

		public static AbonentModel GlobalSelectAbonent { get; set; }
		public static string GlobalSelectGroup { get; set; }
	}
}
