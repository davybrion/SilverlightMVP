using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.Presenters;
using SilverlightMVP.Client.Views;
using SilverlightMVP.Common.Dtos;

namespace Silverlight.ClientTests.Presenters.UserGroupDetailPresenterTests
{
	[TestClass]
	public class CancelFixture : PresenterFixture<UserGroupDetailPresenter, IUserGroupDetailsView>
	{
		protected override UserGroupDetailPresenter CreatePresenter()
		{
			return new UserGroupDetailPresenter(ViewMock.Object, EventAggregatorStub, RequestDispatcherFactoryStub);
		}

		[TestMethod]
		public void RevertsToOriginalValues()
		{
			var userGroup = new UserGroupDto {Id = Guid.NewGuid(), Name = "some name"};
			Presenter.BindingModel.Populate(new UserGroupDto[0], userGroup);
			Presenter.BindingModel.Name = "some other name";

			Presenter.Cancel();

			Assert.AreEqual(userGroup.Name, Presenter.BindingModel.Name);
		}
	}
}