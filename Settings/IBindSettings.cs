using System;
using System.Collections.Generic;
using System.Text;

namespace Settings
{
	public interface IBindSettings
	{
		void Bind(object binder, string section = null);
		T Bind<T>(string section = null) where T : new();
	}
}
