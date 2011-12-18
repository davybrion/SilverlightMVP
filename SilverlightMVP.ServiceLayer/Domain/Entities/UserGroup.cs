using System;
using System.Collections.Generic;

namespace SilverlightMVP.ServiceLayer.Domain.Entities
{
	public class UserGroup : Entity
	{
		private readonly List<UserGroup> children;
		private readonly List<User> users;

		public UserGroup(string name)
		{
			Id = Guid.NewGuid();
			users = new List<User>();
			children = new List<UserGroup>();
			Name = name;
		}

		public string Name { get; set; }
		public UserGroup Parent { get; set; }

		public IEnumerable<User> Users { get { return users.ToArray(); } }
		public IEnumerable<UserGroup> Children { get { return children.ToArray(); } }

		public void AddUser(User user)
		{
			if (users.Contains(user))
			{
				throw new InvalidOperationException("The user already belongs to this group");
			}

			users.Add(user);
		}

		public void RemoveUser(User user)
		{
			if (users.Contains(user))
			{
				users.Remove(user);
			}
		}

		public void AddChildGroup(UserGroup group)
		{
			if (children.Contains(group))
			{
				throw new InvalidOperationException("The user group already belongs to this parent");
			}

			children.Add(group);
			group.Parent = this;
		}

		public void RemoveChildGroup(UserGroup group)
		{
			if (!children.Contains(group))
			{
				throw new InvalidOperationException("The user group is not a child of this parent");
			}

			children.Remove(group);
			group.Parent = null;
		}
	}
}