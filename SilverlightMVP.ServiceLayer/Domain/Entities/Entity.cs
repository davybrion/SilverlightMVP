using System;

namespace SilverlightMVP.ServiceLayer.Domain.Entities
{
	public abstract class Entity
	{
		public Guid Id { get; protected set; }
	}
}