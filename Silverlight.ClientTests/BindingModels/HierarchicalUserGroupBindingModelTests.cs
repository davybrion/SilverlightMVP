using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.BindingModels;

namespace Silverlight.ClientTests.BindingModels
{
	[TestClass]
	public class HierarchicalUserGroupBindingModelTests : BindingModelFixture<HierarchicalUserGroupBindingModel>
	{
		[TestMethod]
		public void SettingNameRaisesPropertyChangedEvent()
		{
			AssertThatPropertyChangesIsTriggeredCorrectly(m => m.Name, "some name");
		}

		[TestMethod]
		public void ChildrenCollectionIsInitializedByDefault()
		{
			Assert.IsNotNull(BindingModel.Children);
		}
	}
}