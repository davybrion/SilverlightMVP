using System;

namespace SilverlightMVP.ServiceLayer.Domain.Entities
{
	public class User : Entity
	{
		public User(string name)
		{
			Id = Guid.NewGuid();
			Name = name;
		}

		public string Name { get; private set; }
	}
}