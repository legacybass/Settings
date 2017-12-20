using Microsoft.Extensions.Configuration;
using Settings.Providers;
using System;
using System.Linq;

namespace Settings
{
	public enum AppSettingsFileType
	{
		NONE,
		JSON,
		XML,
		APP_CONFIG
	}

	public class SettingsReader : IBindSettings, IReadSettings
	{
		protected IConfigurationRoot Configuration { get; set; }

		public SettingsReader(string settingsFile = null, AppSettingsFileType appSettingsFileType = AppSettingsFileType.NONE)
			: this(settingsFile, appSettingsFileType, Environment.GetCommandLineArgs().Skip(1).ToArray())
		{ }

		public SettingsReader(string settingsFile, AppSettingsFileType appSettingsFileType, string[] args)
		{
			// Order is important here. We want the most general (environment) to be loaded first, and the most specific (command line args) to be loaded last.
			var settings = new ConfigurationBuilder()
			.AddEnvironmentVariables();

			if (!string.IsNullOrWhiteSpace(settingsFile))
			{
				if (appSettingsFileType == AppSettingsFileType.NONE)
				{
					var extension = System.IO.Path.GetExtension(settingsFile);
					if (string.Compare(extension, ".json", true) == 0)
						appSettingsFileType = AppSettingsFileType.JSON;
					else if (string.Compare(extension, ".config", true) == 0)
						appSettingsFileType = AppSettingsFileType.APP_CONFIG;
					else if (string.Compare(extension, ".xml", true) == 0)
						appSettingsFileType = AppSettingsFileType.XML;
				}

				if (appSettingsFileType == AppSettingsFileType.JSON)
					settings = settings.AddJsonFile(settingsFile);
				else if (appSettingsFileType == AppSettingsFileType.XML)
					settings = settings.AddXmlFile(settingsFile);
				else if (appSettingsFileType == AppSettingsFileType.APP_CONFIG)
					settings = settings.AddAppConfig(settingsFile);
			}

			settings = settings.AddCommandLine(args);

			Configuration = settings.Build();
		}

		public T Get<T>(string key) => Configuration.GetValue<T>(key);

		public void Bind(object binder, string section = null)
		{
			if (string.IsNullOrWhiteSpace(section))
				Configuration.Bind(binder);
			else
				Configuration.GetSection(section).Bind(binder);
		}

		public T Bind<T>(string section = null) where T : new()
		{
			T obj = new T();
			Bind(obj, section);
			return obj;
		}
	}
}
