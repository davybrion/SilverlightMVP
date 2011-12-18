using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.BindingModels;
using SilverlightMVP.Common.Dtos;

namespace Silverlight.ClientTests.BindingModels
{
	[TestClass]
	public class UserGroupsBindingModelTests : BindingModelFixture<UserGroupsBindingModel>
	{
		private UserGroupDto topLevel1;
		private UserGroupDto secondLevel1;
		private UserGroupDto secondLevel2;
		private UserGroupDto thirdLevel1;
		private UserGroupDto topLevel2;

		protected override void AfterSetUp()
		{
			topLevel1 = new UserGroupDto { Id = Guid.NewGuid(), Name = "topLevel1" };
			secondLevel1 = new UserGroupDto { Id = Guid.NewGuid(), Name = "secondLevel1", ParentId = topLevel1.Id };
			secondLevel2 = new UserGroupDto { Id = Guid.NewGuid(), Name = "secondLevel2", ParentId = topLevel1.Id };
			thirdLevel1 = new UserGroupDto { Id = Guid.NewGuid(), Name = "thirdLevel1", ParentId = secondLevel2.Id };
			topLevel2 = new UserGroupDto { Id = Guid.NewGuid(), Name = "topLevel2" };

			// the order is sorta randomized to make sure the Populate method correctly deals with it
			// (as in: when a child is located in the result before its parent)
			var dtos = new[] { thirdLevel1, secondLevel1, topLevel2, secondLevel2, topLevel1 };

			BindingModel.PopulateFrom(dtos);
		}

		[TestMethod]
		public void TopLevelElementsAreCorrectlyPopulated()
		{
			Assert.AreEqual(2, BindingModel.UserGroups.Count);
			Assert.AreEqual(topLevel2.Id, BindingModel.UserGroups[0].Id);
			Assert.AreEqual(topLevel1.Id, BindingModel.UserGroups[1].Id);
		}

		[TestMethod]
		public void TopLevelElementWithoutChildrenHasEmptyChildrenCollection()
		{
			// the first top level group in the result was topLevel2, which has no children
			Assert.AreEqual(0, BindingModel.UserGroups[0].Children.Count);
		}

		[TestMethod]
		public void TopLevelElementWithChildrenHasCorrectNumberOfChildrenInItsCollection()
		{
			// the second top level group in the result was topLevel1, which has 2 children
			Assert.AreEqual(2, BindingModel.UserGroups[1].Children.Count);
		}

		[TestMethod]
		public void SecondLevelInHierarchyIsCorrectlyPopulated()
		{
			var topLevelWithChildren = BindingModel.UserGroups[1];
			var secondLevel1Model = topLevelWithChildren.Children[0];
			Assert.AreEqual(secondLevel1.Id, secondLevel1Model.Id);
			var secondLevel2Model = topLevelWithChildren.Children[1];
			Assert.AreEqual(secondLevel2.Id, secondLevel2Model.Id);
			Assert.AreEqual(1, secondLevel2Model.Children.Count);
		}

		[TestMethod]
		public void ThirdLevelInHierarchyIsCorrectlyPopulated()
		{
			var thirdLevelModel = BindingModel.UserGroups[1].Children[1].Children[0];
			Assert.AreEqual(thirdLevel1.Id, thirdLevelModel.Id);
		}

		[TestMethod]
		public void AddUserGroup_WithoutParent_IsAddedAsRoot()
		{
			var newId = Guid.NewGuid();
			BindingModel.AddUserGroup(newId, "blah");
			Assert.IsTrue(BindingModel.UserGroups.Any(u => u.Id == newId));
		}

		[TestMethod]
		public void AddUserGroup_WithParent_IsAddedAsChildOfParent()
		{
			var newId = Guid.NewGuid();
			BindingModel.AddUserGroup(newId, "blah", thirdLevel1.Id);
			Assert.AreEqual(newId, BindingModel.UserGroups[1].Children[1].Children[0].Children[0].Id);
		}

		[TestMethod]
		public void UpdateUserGroup_WithoutParent_UserGroupIsModified()
		{
			BindingModel.UpdateUserGroup(topLevel1.Id, "some new name", topLevel1.ParentId);
			Assert.AreEqual("some new name", BindingModel.UserGroups[1].Name);
		}

		[TestMethod]
		public void UpdateUserGroup_WithParent_ChildIsModified()
		{
			BindingModel.UpdateUserGroup(thirdLevel1.Id, "some new name", thirdLevel1.ParentId);
			Assert.AreEqual("some new name", BindingModel.UserGroups[1].Children[1].Children[0].Name);
		}

		[TestMethod]
		public void UpdateUserGroup_WithoutParentAndParentIsNowSet_AddsFormerRootToCorrectParent()
		{
			BindingModel.UpdateUserGroup(topLevel2.Id, "name", topLevel1.Id);
			Assert.AreEqual(1, BindingModel.UserGroups.Count);
			Assert.AreNotEqual(topLevel2.Id, BindingModel.UserGroups[0].Id);
			Assert.IsTrue(BindingModel.UserGroups[0].Children.Any(c => c.Id == topLevel2.Id));
		}

		[TestMethod]
		public void UpdateUserGroup_WithParentAndParentIsNowDifferent_MovesChildFromFormerParentToCorrectOne()
		{
			BindingModel.UpdateUserGroup(secondLevel1.Id, "name", topLevel2.Id);
			Assert.AreEqual(secondLevel1.Id, BindingModel.UserGroups[0].Children[0].Id);
			Assert.IsFalse(BindingModel.UserGroups[1].Children.Any(c => c.Id == secondLevel1.Id));
		}

		[TestMethod]
		public void UpdateUserGroup_WithParentAndParentIsNowNull_MovesChildFromParentToRoot()
		{
			BindingModel.UpdateUserGroup(secondLevel2.Id, "name", null);
			Assert.AreEqual(secondLevel2.Id, BindingModel.UserGroups[2].Id);
			Assert.IsFalse(BindingModel.UserGroups[1].Children.Any(c => c.Id == secondLevel2.Id));
		}

		[TestMethod]
		public void RemoveUserGroup_WithoutParent_UserGroupIsRemoved()
		{
			BindingModel.RemoveUserGroup(topLevel1.Id);
			Assert.IsFalse(BindingModel.UserGroups.Any(u => u.Id == topLevel1.Id));
		}

		[TestMethod]
		public void RemoveUserGroup_WithParent_ChildIsRemoved()
		{
			BindingModel.RemoveUserGroup(thirdLevel1.Id);
			Assert.AreEqual(0, BindingModel.UserGroups[1].Children[1].Children.Count);
		}
	}
}