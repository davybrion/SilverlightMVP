using System;
using System.Collections.Generic;
using System.Linq;
using SilverlightMVP.ServiceLayer.Domain.Entities;

namespace SilverlightMVP.ServiceLayer.Domain
{
	public interface IRepository
	{
		IEnumerable<TEntity> GetAll<TEntity>() where TEntity : Entity;
		TEntity Get<TEntity>(Guid id) where TEntity : Entity;
		void Attach<TEntity>(TEntity entity) where TEntity : Entity;
		void Remove<TEntity>(TEntity entity) where TEntity : Entity;
	}

	public class Repository : IRepository
	{
		private static Dictionary<Guid, Entity> users = CreateUsers();
		private static Dictionary<Guid, Entity> userGroups = CreateUserGroups();

		private static Dictionary<Type, Dictionary<Guid, Entity>> entities = new Dictionary<Type, Dictionary<Guid, Entity>>
		                                                                     {
		                                                                     	{typeof(User), users}, {typeof(UserGroup), userGroups}
		                                                                     };

		public IEnumerable<TEntity> GetAll<TEntity>() where TEntity : Entity
		{
			return entities[typeof(TEntity)].Values.Cast<TEntity>().ToArray();
		}

		public TEntity Get<TEntity>(Guid id) where TEntity : Entity
		{
			return (TEntity)entities[typeof(TEntity)][id];
		}

		public void Attach<TEntity>(TEntity entity) where TEntity : Entity
		{
			entities[typeof(TEntity)].Add(entity.Id, entity);
		}

		public void Remove<TEntity>(TEntity entity) where TEntity : Entity
		{
			entities[typeof(TEntity)].Remove(entity.Id);
		}

		private static Dictionary<Guid, Entity> CreateUsers()
		{
			var users = new[]
			            {
			            	new User("Tom"), new User("Kim"), new User("Frank"), new User("Dick"), new User("Ellen"),
							new User("Harry"), new User("Peter"), new User("John"), new User("Mary"), new User("Chris") 
						};

			return new Dictionary<Guid, Entity>
			       {
			       	{users[0].Id, users[0]},
			       	{users[1].Id, users[1]},
			       	{users[2].Id, users[2]},
			       	{users[3].Id, users[3]},
			       	{users[4].Id, users[4]},
			       	{users[5].Id, users[5]},
			       	{users[6].Id, users[6]},
			       	{users[7].Id, users[7]},
			       	{users[8].Id, users[8]},
			       	{users[9].Id, users[9]}
			       };
		}

		private static Dictionary<Guid, Entity> CreateUserGroups()
		{
			var domainAdmins = new UserGroup("Domain admins");
			var domainUsers = new UserGroup("Domain users");
			var accounting = new UserGroup("Accounting");
			var sales = new UserGroup("Sales");

			domainUsers.AddChildGroup(accounting);
			domainUsers.AddChildGroup(sales);

			return new Dictionary<Guid, Entity>
			       {
			       	{domainAdmins.Id, domainAdmins},
			       	{domainUsers.Id, domainUsers},
			       	{accounting.Id, accounting},
			       	{sales.Id, sales}
			       };
		}
	}
}