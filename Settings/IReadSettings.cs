using System;
using System.Collections.Generic;
using System.Text;

namespace Settings
{
	public interface IReadSettings
	{
		T Get<T>(string key);
	}
}
