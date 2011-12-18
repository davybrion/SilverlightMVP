using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.BindingModels;
using SilverlightMVP.Common.Dtos;

namespace Silverlight.ClientTests.BindingModels
{
	[TestClass]
	public class UserGroupDetailBindingModelTests : BindingModelFixture<UserGroupDetailBindingModel>
	{
		[TestMethod]
		public void SuitableParentUserGroupsCollectionIsCorrectlyInitialized()
		{
			Assert.AreEqual(1, BindingModel.SuitableParentUserGroups.Count);
			Assert.AreEqual(Guid.Empty, BindingModel.SuitableParentUserGroups[0].Id);
			Assert.AreEqual("None", BindingModel.SuitableParentUserGroups[0].Name);
		}

		[TestMethod]
		public void SelectedParentUserGroupIsSetToNoneByDefault()
		{
			Assert.AreEqual(BindingModel.SelectedParentUserGroup, BindingModel.SuitableParentUserGroups[0]);
		}

		[TestMethod]
		public void SettingSelectedParentUserGroupRaisesPropertyChangedEvent()
		{
			AssertThatPropertyChangesIsTriggeredCorrectly(m => m.SelectedParentUserGroup, new UserGroupDto());
		}

		[TestMethod]
		public void SettingIdRaisesCorrectPropertyChangedEvents()
		{
			AssertThatPropertyChangesIsTriggeredCorrectly(m => m.Id, Guid.NewGuid(), true, m => m.IsExistingUserGroup);
		}

		[TestMethod]
		public void SettingNameRaisesPropertyChangedEvent()
		{
			AssertThatPropertyChangesIsTriggeredCorrectly(m => m.Name, "some name");
		}

		[TestMethod]
		public void SettingNameToInvalidValueCausesValidationError()
		{
			BindingModel.Name = null;
			AssertHasErrorMessageForProperty(m => m.Name, "name is a required field");
		}

		[TestMethod]
		public void Populate_AddsSuitableParentUserGroupsToSuitableParentUserGroupsCollection()
		{
			var suitableParentDtos = new[]
			                         {
			                         	new UserGroupDto {Id = Guid.NewGuid(), Name = "Option1"},
			                         	new UserGroupDto {Id = Guid.NewGuid(), Name = "Option2"}
			                         };

			BindingModel.Populate(suitableParentDtos);

			Assert.AreEqual(3, BindingModel.SuitableParentUserGroups.Count); // three because of the default "None" option
			Assert.AreEqual(suitableParentDtos[0].Id, BindingModel.SuitableParentUserGroups[1].Id);
			Assert.AreEqual(suitableParentDtos[1].Id, BindingModel.SuitableParentUserGroups[2].Id);
			Assert.AreEqual(suitableParentDtos[0].Name, BindingModel.SuitableParentUserGroups[1].Name);
			Assert.AreEqual(suitableParentDtos[1].Name, BindingModel.SuitableParentUserGroups[2].Name);
		}

		[TestMethod]
		public void Populate_SetsIdAndNameProperties()
		{
			var currentUserGroup = new UserGroupDto { Id = Guid.NewGuid(), Name = "group1"};
			BindingModel.Populate(new UserGroupDto[0], currentUserGroup);

			Assert.AreEqual(currentUserGroup.Id, BindingModel.Id);
			Assert.AreEqual(currentUserGroup.Name, BindingModel.Name);
		}

		[TestMethod]
		public void Populate_DoesNotChangeSelectedParentUserGroupIfCurrentGroupDoesntHaveParent()
		{
			BindingModel.Populate(new UserGroupDto[0], new UserGroupDto());
			Assert.AreEqual(BindingModel.SelectedParentUserGroup, BindingModel.SuitableParentUserGroups[0]);
		}

		[TestMethod]
		public void Populate_SetsSelectedParentUserGroupIfCurrentGroupHasParent()
		{
			var currentUserGroup = new UserGroupDto {Id = Guid.NewGuid(), ParentId = Guid.NewGuid()};
			var suitableParents = new[] {new UserGroupDto {Id = currentUserGroup.ParentId.Value}};
			BindingModel.Populate(suitableParents, currentUserGroup);

			Assert.AreEqual(currentUserGroup.ParentId.Value, BindingModel.SelectedParentUserGroup.Id);
		}

		[TestMethod]
		public void RevertToOriginalValues_SetsIdAndNameToOriginalValues()
		{
			var userGroup = new UserGroupDto {Id = Guid.NewGuid(), Name = "some name"};
			BindingModel.Populate(new UserGroupDto[0], userGroup);
			BindingModel.Id = Guid.NewGuid();
			BindingModel.Name = "some other name";

			BindingModel.RevertToOriginalValues();

			Assert.AreEqual(userGroup.Id, BindingModel.Id);
			Assert.AreEqual(userGroup.Name, BindingModel.Name);
		}

		[TestMethod]
		public void RevertToOriginalValues_SetsSelectedUserGroupBackToDefaultIfUserGroupOriginallyHadNoParent()
		{
			var userGroup = new UserGroupDto {Id = Guid.NewGuid(), Name = "some name"};
			var suitableParentGroups = new[] {new UserGroupDto {Id = Guid.NewGuid(), Name = "some parent"}};
			BindingModel.Populate(suitableParentGroups, userGroup);
			BindingModel.SelectedParentUserGroup = BindingModel.SuitableParentUserGroups[1];

			BindingModel.RevertToOriginalValues();

			Assert.AreEqual(BindingModel.SelectedParentUserGroup, BindingModel.SuitableParentUserGroups[0]);
		}

		[TestMethod]
		public void RevertToOriginalValues_SetsSelectedUserGroupBackToOriginalParentUserGroup()
		{
			var suitableParentGroups = new[] {new UserGroupDto {Id = Guid.NewGuid(), Name = "some parent"}};
			var userGroup = new UserGroupDto {Id = Guid.NewGuid(), Name = "some name", ParentId = suitableParentGroups[0].Id};
			BindingModel.Populate(suitableParentGroups, userGroup);
			BindingModel.SelectedParentUserGroup = BindingModel.SuitableParentUserGroups[0];

			BindingModel.RevertToOriginalValues();

			Assert.AreEqual(BindingModel.SelectedParentUserGroup, BindingModel.SuitableParentUserGroups[1]);
		}

		[TestMethod]
		public void Clear_SetsEverythingBackToItsInitialState()
		{
			var suitableParentGroups = new[] { new UserGroupDto { Id = Guid.NewGuid(), Name = "some parent" } };
			var userGroup = new UserGroupDto { Id = Guid.NewGuid(), Name = "some name", ParentId = suitableParentGroups[0].Id };
			BindingModel.Populate(suitableParentGroups, userGroup);

			BindingModel.Clear();

			Assert.IsNull(BindingModel.Id);
			Assert.IsNull(BindingModel.Name);

			Assert.AreEqual(1, BindingModel.SuitableParentUserGroups.Count);
			Assert.AreEqual(Guid.Empty, BindingModel.SuitableParentUserGroups[0].Id);
			Assert.AreEqual("None", BindingModel.SuitableParentUserGroups[0].Name);
		}
	}
}