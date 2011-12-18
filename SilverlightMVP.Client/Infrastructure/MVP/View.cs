using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SilverlightMVP.Client.Infrastructure.MVP
{
	public interface IView : IDisposable
	{
		void Hide();
		void Show();
	}

	public class View : UserControl, IView
	{
		private dynamic presenter;

		private static readonly bool inDesignMode = 
			(null == Application.Current) || Application.Current.GetType() == typeof(Application);

		protected TPresenter CreateAndInitializePresenter<TPresenter>()
		{
			if (inDesignMode)
			{
				return default(TPresenter);
			}

			presenter = IoC.Container.Resolve<TPresenter>(new Dictionary<string, object> { { "view", this } });
			presenter.Initialize();
			DataContext = presenter.BindingModel;
			return presenter;
		}

		public void Hide()
		{
			Visibility = Visibility.Collapsed;
		}

		public void Show()
		{
			Visibility = Visibility.Visible;
		}

		public void Dispose()
		{
			// naive implementation of Dispose, but then again, this is just a sample
			IoC.Container.Release(presenter);
			CleanUpChildrenOf(this);
		}

		private void CleanUpChildrenOf(object obj)
		{
			var dependencyObject = obj as DependencyObject;

			if (dependencyObject != null)
			{
				var count = VisualTreeHelper.GetChildrenCount(dependencyObject);

				for (int i = 0; i < count; i++)
				{
					var child = VisualTreeHelper.GetChild(dependencyObject, i);

					if (child != null)
					{
						DisposeIfDisposable(child);
						StopIfProgressBar(child);
						CleanUpChildrenOf(child);
					}
				}
			}
		}

		private static void DisposeIfDisposable(DependencyObject child)
		{
			var disposable = child as IDisposable;
			if (disposable != null) disposable.Dispose();
		}

		private static void StopIfProgressBar(DependencyObject child)
		{
			var progressBar = child as ProgressBar;
			if (progressBar != null) progressBar.IsIndeterminate = false;
		}
	}
}