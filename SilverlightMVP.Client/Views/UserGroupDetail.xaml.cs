using System.Windows;
using SilverlightMVP.Client.Infrastructure.MVP;
using SilverlightMVP.Client.Presenters;

namespace SilverlightMVP.Client.Views
{
	public interface IUserGroupDetailsView : IView
	{
		void PreventDeletion();
		void PreventModification();
		void EnableEverything();
	}

	public partial class UserGroupDetail : IUserGroupDetailsView
	{
		private readonly UserGroupDetailPresenter presenter;

		public UserGroupDetail()
		{
			InitializeComponent();
			presenter = CreateAndInitializePresenter<UserGroupDetailPresenter>();
		}

		public void EnableEverything()
		{
			DeleteButton.Visibility = Visibility.Visible;
			CancelButton.Visibility = Visibility.Visible;
			SaveButton.Visibility = Visibility.Visible;
			NameTextBox.IsEnabled = true;
			SuitableParentUserGroupsComboBox.IsEnabled = true;
		}

		public void PreventDeletion()
		{
			DeleteButton.Visibility = Visibility.Collapsed;
		}

		public void PreventModification()
		{
			NameTextBox.IsEnabled = false;
			SuitableParentUserGroupsComboBox.IsEnabled = false;
			CancelButton.Visibility = Visibility.Collapsed;
			SaveButton.Visibility = Visibility.Collapsed;
		}

		private void DeleteButton_Click(object sender, RoutedEventArgs e)
		{
			presenter.Delete();
		}

		private void CancelButton_Click(object sender, RoutedEventArgs e)
		{
			presenter.Cancel();
		}

		private void SaveButton_Click(object sender, RoutedEventArgs e)
		{
			presenter.PersistChanges();
		}
	}
}
