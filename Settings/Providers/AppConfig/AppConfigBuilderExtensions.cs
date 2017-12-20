using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Settings.Providers
{
	public static class AppConfigBuilderExtensions
	{
		public static IConfigurationBuilder AddAppConfig(this IConfigurationBuilder builder, string path)
		{
			return AddAppConfig(builder, path, optional: false, reloadOnChange: false);
		}

		public static IConfigurationBuilder AddAppConfig(this IConfigurationBuilder builder, string path, bool optional)
		{
			return AddAppConfig(builder, path, optional, reloadOnChange: false);
		}

		public static IConfigurationBuilder AddAppConfig(this IConfigurationBuilder builder, string path, bool optional, bool reloadOnChange)
		{
			return AddAppConfig(builder, provider: null, path: path, optional: optional, reloadOnChange: reloadOnChange);
		}

		public static IConfigurationBuilder AddAppConfig(this IConfigurationBuilder builder, IFileProvider provider, string path, bool optional, bool reloadOnChange)
		{
			if (provider == null && Path.IsPathRooted(path))
			{
				provider = new PhysicalFileProvider(Path.GetDirectoryName(path));
				path = Path.GetFileName(path);
			}

			var source = new AppConfigSource
			{
				FileProvider = provider,
				Path = path,
				Optional = optional,
				ReloadOnChange = reloadOnChange
			};

			builder.Add(source);
			return builder;
		}
	}
}
