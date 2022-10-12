using System;
using System.Windows;

namespace Phone_Book.Infrastructure.WindowClose
{
	public class WindowCloser : DependencyObject
	{
		public static bool GetEnableWindowClosing(DependencyObject obj)
		{
			return (bool)obj.GetValue(EnableWindowClosingProperty);
		}

		public static void SetEnableWindowClosing(DependencyObject obj, bool value)
		{
			obj.SetValue(EnableWindowClosingProperty, value);
		}

		public static readonly DependencyProperty EnableWindowClosingProperty =
			DependencyProperty.RegisterAttached("EnableWindowClosing", typeof(bool), typeof(WindowCloser), new PropertyMetadata(false, OnEnableWindowCloseChanged));

		public static void OnEnableWindowCloseChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			if (d is Window window)
			{
				window.Loaded += (s, e) =>
				{
					if (window.DataContext is IWindowClose wc)
					{
						wc.Close += () =>
						{
							window.Close();
						};

						window.Closing += (s, e) =>
						{
							e.Cancel = !wc.CanClose();
						};
					}
				};
			}
		}
	}
}
