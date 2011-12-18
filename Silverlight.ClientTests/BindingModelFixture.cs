using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SilverlightMVP.Client.Infrastructure.MVP;

namespace Silverlight.ClientTests
{
	public abstract class BindingModelFixture<TBindingModel>
		where TBindingModel : BindingModel<TBindingModel>
	{
		public TBindingModel BindingModel { get; protected set; }

		[TestInitialize]
		public void SetUp()
		{
			BindingModel = CreateBindingModel();
			AfterSetUp();
		}

		protected virtual void AfterSetUp() {}

		protected virtual TBindingModel CreateBindingModel()
		{
			return Activator.CreateInstance<TBindingModel>();
		}

		protected void AssertThatPropertyChangesIsTriggeredCorrectly<TArgument>(Expression<Func<TBindingModel, TArgument>> property, TArgument value)
		{
			AssertThatPropertyChangesIsTriggeredCorrectly(property, value, true, null);
		}

		protected void AssertThatPropertyChangesIsTriggeredCorrectly<TArgument>(Expression<Func<TBindingModel, TArgument>> property, TArgument value, bool includeOriginalPropertyInCheck,
			params Expression<Func<TBindingModel, object>>[] propertiesToCheck)
		{
			var memberInfo = GetMemberExpression(property).Member;

			var propertyName = memberInfo.Name;

			var propertyNamesToCheck = propertiesToCheck != null ?
				propertiesToCheck.Select(p => GetMemberExpression(p).Member.Name).ToList() :
				new List<string>();

			if (includeOriginalPropertyInCheck)
			{
				propertyNamesToCheck.Add(propertyName);
			}

			BindingModel.PropertyChanged += (s, e) =>
			{
				if (propertyNamesToCheck.Contains(e.PropertyName)) propertyNamesToCheck.Remove(e.PropertyName);
			};

			typeof(TBindingModel).GetProperty(propertyName).SetValue(BindingModel, value, null);

			if (propertyNamesToCheck.Count > 0)
			{
				string errorMessage = "The following properties did not trigger PropertyChanged: ";

				foreach (var propertyThatDidntNotify in propertyNamesToCheck)
				{
					errorMessage = errorMessage + "'" + propertyThatDidntNotify + "', ";
				}

				errorMessage = errorMessage.Substring(0, errorMessage.Length - 2);

				Assert.Fail(errorMessage);
			}
		}

		protected void AssertHasErrorMessageForProperty(TBindingModel model, Expression<Func<TBindingModel, object>> expression, string message)
		{
			var memberInfo = GetMemberExpression(expression).Member;
			var propertyName = memberInfo.Name;
			Assert.IsTrue(model.GetErrors(propertyName).Cast<string>().Any(e => e == message));
		}

		protected void AssertHasNoErrorMessageForProperty(TBindingModel model, Expression<Func<TBindingModel, object>> expression, string message)
		{
			var memberInfo = GetMemberExpression(expression).Member;
			var propertyName = memberInfo.Name;
			Assert.IsFalse(model.GetErrors(propertyName).Cast<string>().Any(e => e == message));
		}

		protected void AssertHasErrorMessageForProperty(Expression<Func<TBindingModel, object>> expression, string message)
		{
			AssertHasErrorMessageForProperty(BindingModel, expression, message);
		}

		protected void AssertHasNoErrorMessageForProperty(Expression<Func<TBindingModel, object>> expression, string message)
		{
			AssertHasNoErrorMessageForProperty(BindingModel, expression, message);
		}

		protected void AssertThatPropertyValidationIsTriggered<TArgument>(Expression<Func<TBindingModel, TArgument>> property, TArgument value)
		{
			AssertThatPropertyValidationIsTriggered(BindingModel, property, value, true, null);
		}

		protected void AssertThatPropertyValidationIsTriggered<TArgument>(TBindingModel model,
			Expression<Func<TBindingModel, TArgument>> property, TArgument value)
		{
			AssertThatPropertyValidationIsTriggered(model, property, value, true, null);
		}

		protected void AssertThatPropertyValidationIsTriggered<TArgument>(Expression<Func<TBindingModel, TArgument>> property, TArgument value, bool includeOriginalPropertyInCheck,
			params Expression<Func<TBindingModel, object>>[] propertiesToCheck)
		{
			AssertThatPropertyValidationIsTriggered(BindingModel, property, value, includeOriginalPropertyInCheck, propertiesToCheck);
		}

		protected void AssertThatPropertyValidationIsTriggered<TArgument>(TBindingModel model,
			Expression<Func<TBindingModel, TArgument>> property, TArgument value, bool includeOriginalPropertyInCheck,
			params Expression<Func<TBindingModel, object>>[] propertiesToCheck)
		{
			var memberInfo = GetMemberExpression(property).Member;

			var propertyName = memberInfo.Name;

			var propertyNamesToCheck = propertiesToCheck != null ?
				propertiesToCheck.Select(p => GetMemberExpression(p).Member.Name).ToList() :
				new List<string>();

			if (includeOriginalPropertyInCheck)
			{
				propertyNamesToCheck.Add(propertyName);
			}

			model.ErrorsChanged += (s, e) =>
			{
				if (propertyNamesToCheck.Contains(e.PropertyName)) propertyNamesToCheck.Remove(e.PropertyName);
			};

			typeof(TBindingModel).GetProperty(propertyName).SetValue(model, value, null);

			if (propertyNamesToCheck.Count > 0)
			{
				string errorMessage = "The following properties did not Validate: ";

				foreach (var propertyThatDidNotValidate in propertyNamesToCheck)
				{
					errorMessage = errorMessage + "'" + propertyThatDidNotValidate + "', ";
				}

				errorMessage = errorMessage.Substring(0, errorMessage.Length - 2);

				Assert.Fail(errorMessage);
			}
		}

		private static MemberExpression GetMemberExpression<T1, T2>(Expression<Func<T1, T2>> expression)
		{
			MemberExpression memberExpression;

			if (expression.Body is UnaryExpression)
			{
				memberExpression = ((UnaryExpression)expression.Body).Operand as MemberExpression;
			}
			else
			{
				memberExpression = expression.Body as MemberExpression;
			}

			return memberExpression;
		}
	}
}