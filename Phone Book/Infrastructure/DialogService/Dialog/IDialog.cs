using System;
using System.Windows;

namespace Phone_Book.Infrastructure.DialogService
{
	internal interface IDialog
	{
		string OpenImageFile();

		string OpenJsonFile();

		string SaveJsonFile();

		bool ShowMessageBox(string textBody, string header, MessageBoxButton button, MessageBoxImage image);

		void ShowMessageException(string textBody, string header);

		int ShowMessageQuestion(string textBody, string header);

		void ShowMessageInfo(string textBody, string header);
	}
}
