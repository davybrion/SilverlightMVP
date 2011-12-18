using System;
using System.Windows;
using System.Windows.Controls;
using SilverlightMVP.Client.BindingModels;
using SilverlightMVP.Client.Infrastructure.MVP;
using SilverlightMVP.Client.Presenters;

namespace SilverlightMVP.Client.Views
{
	public interface IUserGroupsView : IView
	{
		void SelectItemInTreeView(HierarchicalUserGroupBindingModel userGroupModel);
		void ExpandTreeView();
		void HideAddNewButton();
	}

	public partial class UserGroups : IUserGroupsView
	{
		private readonly UserGroupsPresenter presenter;

		public UserGroups()
		{
			InitializeComponent();
			presenter = CreateAndInitializePresenter<UserGroupsPresenter>();
		}

		public void SelectItemInTreeView(HierarchicalUserGroupBindingModel userGroupModel)
		{
			UserGroupsTreeView.SelectItem(userGroupModel);
		}

		public void ExpandTreeView()
		{
			for (int i = 0; i < UserGroupsTreeView.Items.Count; i++)
			{
				ExpandAllTreeViewItems((TreeViewItem)UserGroupsTreeView.ItemContainerGenerator.ContainerFromIndex(i));
			}
		}

		private void ExpandAllTreeViewItems(TreeViewItem treeViewItem)
		{
			if (!treeViewItem.IsExpanded)
			{
				treeViewItem.IsExpanded = true;
				treeViewItem.Dispatcher.BeginInvoke(() => ExpandAllTreeViewItems(treeViewItem));
			}
			else
			{
				for (int i = 0; i < treeViewItem.Items.Count; i++)
				{
					var child = (TreeViewItem)treeViewItem.ItemContainerGenerator.ContainerFromIndex(i);
					ExpandAllTreeViewItems(child);
				}
			}
		}

		public void HideAddNewButton()
		{
			AddNewButton.Visibility = Visibility.Collapsed;
		}

		private void UserGroupsTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
		{
			presenter.DealWithSelectedUserGroup((HierarchicalUserGroupBindingModel)e.NewValue);
		}

		private void AddNewButton_Click(object sender, RoutedEventArgs e)
		{
			presenter.PrepareUserGroupCreation();
		}
	}
}
