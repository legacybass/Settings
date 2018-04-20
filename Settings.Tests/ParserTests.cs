using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Settings.Tests.Models;
using Should;

namespace Settings.Tests
{
	[TestClass]
	public class ParserTests
	{
		protected SettingsReader SettingsReader { get; set; }

		[TestMethod]
		public void SettingsReader_ReadsFromJsonConfigFile()
		{
			string path = "app.json";
			string expectedString = "stringvalue";
			int expectedInt = 42;

			SettingsReader = new SettingsReader(path, AppSettingsFileType.JSON);

			var actualString = SettingsReader.Get<string>("jsonString");
			var actualInt = SettingsReader.Get<int>("jsonInt");

			actualString.ShouldEqual(expectedString);
			actualInt.ShouldEqual(expectedInt);
		}

		[TestMethod]
		public void SettingsReader_ReadSettingsFromAppConfigFile()
		{
			string path = "App.config";
			string expectedString = "teststringvalue";
			int expectedInt = 42;

			SettingsReader = new SettingsReader(path, AppSettingsFileType.APP_CONFIG);

			var actualString = SettingsReader.Get<string>("stringKey");
			var actualInt = SettingsReader.Get<int>("intKey");

			actualString.ShouldEqual(expectedString);
			actualInt.ShouldEqual(expectedInt);
		}

		[TestMethod]
		public void SettingsReader_ReadConnectionStringsFromAppConfigFile()
		{
			string path = "App.config";
			string connectionName = "MyTestDb";
			string expectedString = "data source=(local);initial catalog=Test;integrated security=True;MultipleActiveResultSets=True;";

			SettingsReader = new SettingsReader(path, AppSettingsFileType.APP_CONFIG);

			var actualString = SettingsReader.Get<string>($"ConnectionStrings:{connectionName}");

			actualString.ShouldEqual(expectedString);
		}

		[TestMethod]
		public void SettingsReader_ReadFromXmlFile()
		{
			string path = "app.xml";
			string expectedString = "testStringValue";
			int expectedInt = 42;

			SettingsReader = new SettingsReader(path, AppSettingsFileType.XML);

			var actualString = SettingsReader.Get<string>("stringKey");
			var actualInt = SettingsReader.Get<int>("intKey");

			actualString.ShouldEqual(expectedString);
			actualInt.ShouldEqual(expectedInt);
		}

		[TestMethod]
		public void SettingsReader_ReadFromCommandLine()
		{
			string value1 = "value1",
				value2 = "value2";
			int value3 = 42;

			string key1 = "key1",
				key2 = "key2",
				key3 = "key3";

			var args = new string[] { $"--{key1}", value1, $"--{key2}", value2, $"--{key3}", value3.ToString() };

			SettingsReader = new SettingsReader(null, AppSettingsFileType.APP_CONFIG, args);

			var actualValue1 = SettingsReader.Get<string>(key1);
			var actualValue2 = SettingsReader.Get<string>(key2);
			var actualValue3 = SettingsReader.Get<int>(key3);

			actualValue1.ShouldEqual(value1);
			actualValue2.ShouldEqual(value2);
			actualValue3.ShouldEqual(value3);
		}

		[TestMethod]
		public void SettingsReader_ReadsFromEnvironmentVariables()
		{
			string key1 = "MyCustomKey",
				key2 = "MyCustomInt";

			string value1 = "MyCustomValue";
			int value2 = 42;

			System.Environment.SetEnvironmentVariable(key1, value1);
			System.Environment.SetEnvironmentVariable(key2, value2.ToString());

			SettingsReader = new SettingsReader();

			var actualValue1 = SettingsReader.Get<string>(key1);
			var actualValue2 = SettingsReader.Get<int>(key2);

			actualValue1.ShouldEqual(value1);
			actualValue2.ShouldEqual(value2);
		}

		[TestMethod]
		public void SettingsReader_BindsToModels()
		{
			var user = new TestUser
			{
				Username = "bobthebob",
				Id = 42,
				Email = "bob@bobbington.com",
				Address = new Address
				{
					Street = "123 Fake St.",
					City = "Springfield",
					State = "Hahahahaha",
					Zip = "00000"
				}
			};

			var args = new string[]
			{
				$"--{nameof(user.Username)}", user.Username,
				$"--{nameof(user.Id)}", user.Id.ToString(),
				$"--{nameof(user.Email)}", user.Email,
				$"--Address:{nameof(user.Address.Street)}", user.Address.Street,
				$"--Address:{nameof(user.Address.City)}", user.Address.City,
				$"--Address:State", user.Address.State,
				$"--Address:Zip", user.Address.Zip
			};

			SettingsReader = new SettingsReader(null, AppSettingsFileType.APP_CONFIG, args);

			var boundObject = new TestUser();
			SettingsReader.Bind(boundObject);

			boundObject.Username.ShouldEqual(user.Username);
			boundObject.Id.ShouldEqual(user.Id);
			boundObject.Email.ShouldEqual(user.Email);
			boundObject.Address.ShouldNotBeNull();
			boundObject.Address.Street.ShouldEqual(user.Address.Street);
			boundObject.Address.City.ShouldEqual(user.Address.City);
			boundObject.Address.State.ShouldEqual(user.Address.State);
			boundObject.Address.Zip.ShouldEqual(user.Address.Zip);
		}

		[TestMethod]
		public void SettingsReader_BindsToModelsWithGenerics()
		{
			var user = new TestUser
			{
				Username = "bobthebob",
				Id = 42,
				Email = "bob@bobbington.com",
				Address = new Address
				{
					Street = "123 Fake St.",
					City = "Springfield",
					State = "Hahahahaha",
					Zip = "00000"
				}
			};

			var args = new string[]
			{
				$"--{nameof(user.Username)}", user.Username,
				$"--{nameof(user.Id)}", user.Id.ToString(),
				$"--{nameof(user.Email)}", user.Email,
				$"--Address:{nameof(user.Address.Street)}", user.Address.Street,
				$"--Address:{nameof(user.Address.City)}", user.Address.City,
				$"--Address:State", user.Address.State,
				$"--Address:Zip", user.Address.Zip
			};

			SettingsReader = new SettingsReader(null, AppSettingsFileType.APP_CONFIG, args);

			var boundObject = SettingsReader.Bind<TestUser>();

			boundObject.Username.ShouldEqual(user.Username);
			boundObject.Id.ShouldEqual(user.Id);
			boundObject.Email.ShouldEqual(user.Email);
			boundObject.Address.ShouldNotBeNull();
			boundObject.Address.Street.ShouldEqual(user.Address.Street);
			boundObject.Address.City.ShouldEqual(user.Address.City);
			boundObject.Address.State.ShouldEqual(user.Address.State);
			boundObject.Address.Zip.ShouldEqual(user.Address.Zip);
		}

		[TestMethod]
		public void SettingsReader_SettingsAreOverwrittenByMoreSpecificSettings()
		{
			string environmentValue1 = "environment value 1",
				environmentValue2 = "environment value 2",
				environmentValue3 = "environment value 3",
				appSettingsValue2 = "app value 2",
				cliValue1 = "cli value 1";

			string key1 = "key1",
				key2 = "key2",
				key3 = "key3";

			var args = new string[] { $"--{key1}", cliValue1 };

			System.Environment.SetEnvironmentVariable(key1, environmentValue1);
			System.Environment.SetEnvironmentVariable(key2, environmentValue2);
			System.Environment.SetEnvironmentVariable(key3, environmentValue3);

			SettingsReader = new SettingsReader("app.json", AppSettingsFileType.JSON, args);

			var actualValue1 = SettingsReader.Get<string>(key1);
			var actualValue2 = SettingsReader.Get<string>(key2);
			var actualValue3 = SettingsReader.Get<string>(key3);

			actualValue1.ShouldEqual(cliValue1);
			actualValue2.ShouldEqual(appSettingsValue2);
			actualValue3.ShouldEqual(environmentValue3);
		}
	}
}
