using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Settings.Providers
{
	internal class AppConfigFileParser
	{
		protected IDictionary<string, string> Data { get; }
		protected Stack<string> Context { get; }

		public AppConfigFileParser()
		{
			Data = new SortedDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
			Context = new Stack<string>();
		}

		public IDictionary<string, string> Parse(Stream input)
		{
			Data.Clear();
			Context.Clear();

			var doc = XDocument.Load(input);
			Parse(doc.Root);

			return Data;
		}

		protected void Parse(XElement root)
		{
			var appSettings = root.Descendants().SingleOrDefault(d => d.Name == "appSettings");
			if (appSettings != null)
			{
				foreach (var setting in appSettings.Elements())
				{
					// TODO: For now I'm adding this twice since we're only parsing the appSettings information. However, in the future
					// we should be following the convention of "section:subsection:key" for retrieving values.
					Data.Add($"appSettings:{setting.Attribute("key").Value}", setting.Attribute("value").Value);
					Data.Add(setting.Attribute("key").Value, setting.Attribute("value").Value);
				}
			}

			var connectionStrings = root.Descendants().SingleOrDefault(d => d.Name == "connectionStrings");

			if (connectionStrings != null)
			{
				foreach (var connection in connectionStrings.Elements())
				{
					Data.Add($"connectionStrings:{connection.Attribute("name").Value}", connection.Attribute("connectionString").Value);
				}
			}
		}
	}
}
