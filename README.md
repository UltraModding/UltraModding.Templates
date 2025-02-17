# UltraModding.Templates

A collection of .NET project templates for Ultrakill plugins using BepInEx 5.

## Installation

You will need [.NET 6 or newer](https://dotnet.microsoft.com/download) to use the templates.

To install, use [dotnet](https://dotnet.microsoft.com/download) tool.

### Install stable templates

These templates are for **BepInEx 5**:

> For now, the package isn't uploaded to NuGet, so you'll have to download the project as a zip and use the following command from within the solution:
```
dotnet new install ./UltraModding.Templates/templates/
```

This will install the following template:

| Templates                                    | Short Name           | Language   | Tags                                   |
| -------------------------------------------- | -------------------- | ---------- | -------------------------------------- |
| Ultrakill Minimal Plugin Template            | ukplugin-min         | [C#]       | BepInEx/BepInEx 5/Plugin/Ultrakill     |

## Using a template

To use a template, you can use `dotnet new` command.  
If you use Rider or Visual Studio 2022, you will be able to select the templates when creating a new solution.

For example, to create an Ultrakill plugin project:
```
dotnet new ukplugin-min -n MyPluginName
```

This will create a folder name MyPluginName with an example plugin project, and with some dependencies.

All templates have additional options. To view them, use `-h` or `--help` switch. For example:

```
dotnet new bepinex5plugin --help
```

will show additional options you can specify when creating a project:

```
Options:
  -N|--Name                 The name of the plugin
                            text - Optional
                            Default: My first plugin

  -G|--GUID                 The unique GUID of the plugin
                            text - Optional
                            Default: com.yourname.pluginname

  -V|--Version              Plugin version
                            text - Optional
                            Default: 1.0.0

  -U|--UltrakillDirectory   The path to the Ultrakill directory
                            text - Optional
                            Default: C:/Program Files (x86)/Steam/steamapps/common/ULTRAKILL/
```

## Documentation, guides and more

For more guides, refer to [BepInEx Docs](https://docs.bepinex.dev) and the (WIP) [UltraModding Wiki](https://ultramodding.github.io/).  
If you're writing a BepInEx plugin for the first time, [check out the plugin development walkthrough](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/index.html).
