using System;
using System.Collections.ObjectModel;
using SilverlightMVP.Client.Infrastructure.MVP;

namespace SilverlightMVP.Client.BindingModels
{
	public class HierarchicalUserGroupBindingModel : BindingModel<HierarchicalUserGroupBindingModel>
	{
		public HierarchicalUserGroupBindingModel()
		{
			Children = new ObservableCollection<HierarchicalUserGroupBindingModel>();
		}

		public Guid Id { get; set; }

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

		public ObservableCollection<HierarchicalUserGroupBindingModel> Children { get; private set; }
	}
}