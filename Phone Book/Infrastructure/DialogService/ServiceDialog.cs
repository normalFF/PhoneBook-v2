using Microsoft.Win32;
using Phone_Book.Infrastructure.DialogService.Abonents;
using Phone_Book.Infrastructure.DialogService.Groups;
using Phone_Book.View.DialogWindows.AbonentDialogs;
using Phone_Book.View.DialogWindows.GroupDialogs;
using System;
using System.IO;
using System.Windows;

namespace Phone_Book.Infrastructure.DialogService
{
	internal class ServiceDialogs : IDialogService
	{
		void IDialogAbonentService.ShowDialog(EnumDialogsWindowsFromAbonent abonentEnum, Action<bool> actionCallBack)
		{
			switch (abonentEnum)
			{
				case EnumDialogsWindowsFromAbonent.AbonentInfo:
					Show(new AbonentInfo(), actionCallBack);
					break;
				case EnumDialogsWindowsFromAbonent.AddAbonent:
					Show(new AddAbonentWindow(), actionCallBack);
					break;
				case EnumDialogsWindowsFromAbonent.EditAbonent:
					Show(new EditAbonentWindow(), actionCallBack);
					break;
			}
		}

		void IDialogGroupService.ShowDialog(EnumDialogsWindowsFromGroup groupEnum, Action<bool> actionCallBack)
		{
			switch (groupEnum)
			{
				case EnumDialogsWindowsFromGroup.EditGroups:
					Show(new EditGroupWindow(), actionCallBack);
					break;
				case EnumDialogsWindowsFromGroup.AddGroup:
					Show(new AddGroupWindow(), actionCallBack);
					break;
			}
		}

		private void Show(Window dialog, Action<bool> actionCallBack)
		{
			EventHandler closeEventHandler = null;
			closeEventHandler = (s, e) =>
			{
				actionCallBack(dialog.DialogResult.Value);
				dialog.Closed -= closeEventHandler;
			};
			dialog.Closed += closeEventHandler;
			dialog.ShowDialog();
		}

		public bool ShowMessageBox(string textBody, string header, MessageBoxButton button, MessageBoxImage image)
		{
			var messageBoxResult = MessageBox.Show(textBody, header, button, image);

			switch (messageBoxResult)
			{
				case MessageBoxResult.OK:
				case MessageBoxResult.Yes:
					return true;
				default:
					return false;
			}
		}

		public string OpenImageFile()
		{
			OpenFileDialog dialog = new();
			dialog.Multiselect = false;
			dialog.Title = "Выбрать изображение";
			dialog.Filter = "Files|*.jpg;*.jpeg;*.png;";
			dialog.InitialDirectory = Directory.GetCurrentDirectory();
			dialog.ShowDialog();
			return dialog.FileName;
		}

		public string OpenJsonFile()
		{
			OpenFileDialog dialog = new();
			dialog.Multiselect = false;
			dialog.Title = "Выбрать файл";
			dialog.Filter = "Files|*.json";
			dialog.InitialDirectory = Directory.GetCurrentDirectory();
			dialog.ShowDialog();
			return dialog.FileName;
		}

		public string SaveJsonFile()
		{
			SaveFileDialog dialog = new();
			dialog.OverwritePrompt = true;
			dialog.Title = "Выбрать файл";
			dialog.Filter = "Files|*.json";
			dialog.InitialDirectory = Directory.GetCurrentDirectory();
			dialog.ShowDialog();
			return dialog.FileName;
		}

		public void ShowMessageException(string textBody, string header)
		{
			MessageBox.Show(textBody, header, MessageBoxButton.OK, MessageBoxImage.Error);
		}

		public int ShowMessageQuestion(string textBody, string header)
		{
			var result = MessageBox.Show(textBody, header, MessageBoxButton.YesNoCancel, MessageBoxImage.Question);

			switch (result)
			{
				case MessageBoxResult.Yes:
					return 1;
				case MessageBoxResult.No:
					return -1;
				default: return 0;
			}
		}

		public void ShowMessageInfo(string textBody, string header)
		{
			MessageBox.Show(textBody, header, MessageBoxButton.OK, MessageBoxImage.Information);
		}
	}
}
