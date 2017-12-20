using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace Settings.Providers
{
	public class AppConfigProvider : FileConfigurationProvider
	{
		public AppConfigProvider(AppConfigSource source) : base(source)
		{ }

		public override void Load(Stream stream)
		{
			var parser = new AppConfigFileParser();

			Data = parser.Parse(stream);
		}
	}
}
