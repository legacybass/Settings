using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace Settings.Providers
{
	public class AppConfigSource : FileConfigurationSource
	{
		public override IConfigurationProvider Build(IConfigurationBuilder builder)
		{
			FileProvider = FileProvider ?? builder.GetFileProvider();
			return new AppConfigProvider(this);
		}

	}
}
