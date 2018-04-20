# Mindfire Settings Reader

This project is meant to be a simple way of reading settings for an app in all project types that support .NET Standard. It provides a way of having a consistent settings experience across all projects and solutions.

## Obtaining an Instance
The SettingsReader concrete class can be created for any other class that uses it, but the best way to use this reader is through dependency injection. Simply register the interface with the DI container and have it provide the concrete class.
``` C#
container.RegisterSingleton<IReadSettings, SettingsReader>();
```
Then, any class that needs the settings reader may simply ask for it.
``` C#
public MyClass(IReadSettings settingsReader) { this.SettingsReader = settingsReader; }
```
You may also register the `IBindSettings` interface in order to bind settings classes to the data you seek.

## Usage
### Simple Settings
There are a couple of ways to use the reader, both based on which interface is being used. Reading simple settings out of the reader can be done by giving a key and specifying the value type (note: these must be primitive types).
``` C#
ISettingsReader.Get<string>("MySettings");
```
If sectioned values are needed, each section can be separated by a semicolon until the value you want read.
``` C#
ISettingsReader.Get<int>("Services:InventoryService:port");
```

### Complex Settings
You can also retrieve all settings for a section by using the `IBindSettings` interface. Provide a type that has the properties you want read, and the settings reader will attempt to fill the object.<br />
``` C#
public class User {
	public string Username	{ get; set; }
	public int Id { get; set; }
}

var user = new User();
IBindSettings.Bind(user);
```

### Connection Strings
By default, all settings from an app.config file are read from the `appSettings` element. However, you can read from the `connectionStrings` section by prepending the section name with `ConnectionStrings:` like so
``` C#
ISettingsReader.Get<string>("ConnectionStrings:MyDbConnection");
```

See unit tests for more examples.