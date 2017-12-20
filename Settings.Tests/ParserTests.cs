using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Settings.Tests.Models;

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

			Assert.AreEqual(expectedString, actualString);
			Assert.AreEqual(expectedInt, actualInt);
		}

		[TestMethod]
		public void SettingsReader_ReadFromAppConfigFile()
		{
			string path = "App.config";
			string expectedString = "teststringvalue";
			int expectedInt = 42;

			SettingsReader = new SettingsReader(path, AppSettingsFileType.APP_CONFIG);

			var actualString = SettingsReader.Get<string>("stringKey");
			var actualInt = SettingsReader.Get<int>("intKey");

			Assert.AreEqual(expectedString, actualString);
			Assert.AreEqual(expectedInt, actualInt);
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

			Assert.AreEqual(expectedString, actualString);
			Assert.AreEqual(expectedInt, actualInt);
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

			Assert.AreEqual(value1, actualValue1);
			Assert.AreEqual(value2, actualValue2);
			Assert.AreEqual(value3, actualValue3);
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

			Assert.AreEqual(value1, actualValue1);
			Assert.AreEqual(value2, actualValue2);
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

			Assert.AreEqual(user.Username, boundObject.Username);
			Assert.AreEqual(user.Id, boundObject.Id);
			Assert.AreEqual(user.Email, boundObject.Email);
			Assert.IsNotNull(user.Address);
			Assert.AreEqual(user.Address.Street, boundObject.Address.Street);
			Assert.AreEqual(user.Address.City, boundObject.Address.City);
			Assert.AreEqual(user.Address.State, boundObject.Address.State);
			Assert.AreEqual(user.Address.Zip, boundObject.Address.Zip);
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

			Assert.AreEqual(user.Username, boundObject.Username);
			Assert.AreEqual(user.Id, boundObject.Id);
			Assert.AreEqual(user.Email, boundObject.Email);
			Assert.IsNotNull(user.Address);
			Assert.AreEqual(user.Address.Street, boundObject.Address.Street);
			Assert.AreEqual(user.Address.City, boundObject.Address.City);
			Assert.AreEqual(user.Address.State, boundObject.Address.State);
			Assert.AreEqual(user.Address.Zip, boundObject.Address.Zip);
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

			Assert.AreEqual(cliValue1, actualValue1);
			Assert.AreEqual(appSettingsValue2, actualValue2);
			Assert.AreEqual(environmentValue3, actualValue3);
		}
	}
}
