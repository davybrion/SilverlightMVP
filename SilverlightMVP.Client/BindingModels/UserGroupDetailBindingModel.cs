using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using SilverlightMVP.Client.Infrastructure.MVP;
using SilverlightMVP.Common.Dtos;

namespace SilverlightMVP.Client.BindingModels
{
	public class UserGroupDetailBindingModel : BindingModel<UserGroupDetailBindingModel>
	{
		private string originalName;
		private Guid? originalId;
		private UserGroupDto originalSelectedParent;

		public ObservableCollection<UserGroupDto> SuitableParentUserGroups { get; private set; }

		private UserGroupDto selectedParentUserGroup;

		public UserGroupDto SelectedParentUserGroup
		{
			get { return selectedParentUserGroup; }
			set
			{
				selectedParentUserGroup = value;
				NotifyPropertyChanged(m => m.SelectedParentUserGroup);
			}
		}

		private Guid? id;

		public Guid? Id
		{
			get { return id; }
			set
			{
				id = value;
				NotifyPropertyChanged(m => m.Id);
				NotifyPropertyChanged(m => m.IsExistingUserGroup);
			}
		}

		public bool IsExistingUserGroup { get { return id.HasValue && id.Value != Guid.Empty; } }

		private string name;

		public string Name
		{
			get { return name; }
			set
			{
				name = value;
				NotifyPropertyChanged(m => m.Name);
			}
		}

		public UserGroupDetailBindingModel()
		{
			SuitableParentUserGroups = new ObservableCollection<UserGroupDto>();
			Clear();
			AddValidationFor(m => m.Name)
				.When(m => string.IsNullOrWhiteSpace(m.name))
				.WithMessage("name is a required field");
		}

		public void Clear()
		{
			SuitableParentUserGroups.Clear();
			SuitableParentUserGroups.Add(new UserGroupDto { Id = Guid.Empty, Name = "None" });
			SelectedParentUserGroup = SuitableParentUserGroups[0];

			originalId = Id = null;
			originalName = Name = null;
			originalSelectedParent = SelectedParentUserGroup;
		}

		public void Populate(IEnumerable<UserGroupDto> suitableParentUserGroups, UserGroupDto currentUserGroup = null)
		{
			foreach (var suitableParentUserGroup in suitableParentUserGroups)
			{
				SuitableParentUserGroups.Add(suitableParentUserGroup);
			}

			if (currentUserGroup != null)
			{
				originalName = Name = currentUserGroup.Name;
				originalId = Id = currentUserGroup.Id;
				originalSelectedParent = SelectedParentUserGroup;

				if (currentUserGroup.ParentId.HasValue)
				{
					originalSelectedParent = SelectedParentUserGroup = SuitableParentUserGroups.First(u => u.Id == currentUserGroup.ParentId);
				}
			}
		}

		public void RevertToOriginalValues()
		{
			Name = originalName;
			Id = originalId;
			SelectedParentUserGroup = originalSelectedParent;
		}
	}
}