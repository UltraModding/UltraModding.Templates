## BepInEx PluginInfo generator

Generates `MyPluginInfo.cs` based on csproj tags.

## Basic usage

Define the following properties in your `csproj`:

```xml
<GUID>com.example.plugin</GUID>
<Product>Example Plugin</Product>
<Version>1.0.0</Version>
```

this will generate the following class:

```cs
namespace Example.Plugin;

internal static class MyPluginInfo
{
    public const string PLUGIN_GUID = "com.example.plugin";
    public const string PLUGIN_NAME = "Example Plugin";
    public const string PLUGIN_VERSION = "1.0.0";
}
```
