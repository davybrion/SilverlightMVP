using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SilverlightMVP.Client.Infrastructure.MVP;
using SilverlightMVP.Common.Dtos;

namespace SilverlightMVP.Client.BindingModels
{
	public class UserGroupsBindingModel : BindingModel<UserGroupsBindingModel>
	{
		public UserGroupsBindingModel()
		{
			UserGroups = new ObservableCollection<HierarchicalUserGroupBindingModel>();
		}

		public ObservableCollection<HierarchicalUserGroupBindingModel> UserGroups { get; private set; }

		public void PopulateFrom(IEnumerable<UserGroupDto> dtos)
		{
			var dictionary = dtos.ToDictionary(dto => dto.Id, dto => new HierarchicalUserGroupBindingModel {Id = dto.Id, Name = dto.Name});

			foreach (var userGroup in dictionary.Values)
			{
				var parentId = dtos.First(d => d.Id == userGroup.Id).ParentId;

				if (parentId.HasValue)
				{
					dictionary[parentId.Value].Children.Add(userGroup);
				}
			}

			var rootIds = dtos.Where(d => d.ParentId == null).Select(d => d.Id);
			dictionary.Values.Where(u => rootIds.Contains(u.Id)).ToList().ForEach(u => UserGroups.Add(u));
		}

		public HierarchicalUserGroupBindingModel AddUserGroup(Guid id, string name, Guid? parentId = null)
		{
			var newUserGroup = new HierarchicalUserGroupBindingModel { Id = id, Name = name };

			if (!parentId.HasValue)
			{
				UserGroups.Add(newUserGroup);
			}
			else
			{
				FindGroupById(parentId.Value, UserGroups).Children.Add(newUserGroup);
			}

			return newUserGroup;
		}

		private static HierarchicalUserGroupBindingModel FindGroupById(Guid id, IEnumerable<HierarchicalUserGroupBindingModel> usergroups)
		{
			foreach (var usergroup in usergroups)
			{
				if (usergroup.Id == id) return usergroup; 
				var childGroup = FindGroupById(id, usergroup.Children);
				if (childGroup != null) return childGroup;
			}

			return null;
		}

		public void UpdateUserGroup(Guid id, string name, Guid? parentId)
		{
			var group = FindGroupById(id, UserGroups);
			group.Name = name;

			var parent = FindParentOfChildWithId(id, UserGroups);

			if (parent == null)
			{
				if (parentId.HasValue)
				{
					UserGroups.Remove(group);
					FindGroupById(parentId.Value, UserGroups).Children.Add(group);
				}
			}
			else
			{
				if (parentId.HasValue && parent.Id != parentId.Value)
				{
					parent.Children.Remove(group);
					FindGroupById(parentId.Value, UserGroups).Children.Add(group);
				}
				else if (!parentId.HasValue)
				{
					parent.Children.Remove(group);
					UserGroups.Add(group);
				}
			}
		}

		public void RemoveUserGroup(Guid id)
		{
			if (UserGroups.Any(u => u.Id == id))
			{
				UserGroups.Remove(UserGroups.First(u => u.Id == id));
			}
			else
			{
				var parent = FindParentOfChildWithId(id, UserGroups);
				parent.Children.Remove(parent.Children.First(u => u.Id == id));
			}
		}

		private static HierarchicalUserGroupBindingModel FindParentOfChildWithId(Guid id, IEnumerable<HierarchicalUserGroupBindingModel> usergroups)
		{
			foreach (var usergroup in usergroups)
			{
				if (usergroup.Children.Any(u => u.Id == id)) return usergroup; 
				var childGroup = FindParentOfChildWithId(id, usergroup.Children);
				if (childGroup != null) return childGroup;
			}

			return null;
		}
	}
}