# UltraModding.Templates

A collection of .NET project templates for Ultrakill plugins using BepInEx 5.

## Installation

You will need [.NET 6 or newer](https://dotnet.microsoft.com/download) to use the templates.

To install, use [dotnet](https://dotnet.microsoft.com/download) tool.

### Install stable templates

These templates are for **ULTRAKILL Patch 16d or newer**, using **BepInEx 5**.

Firstly, get the latest version of the package from [NuGet](https://www.nuget.org/packages/UltraModding.Templates/)
via the following command (or installing manually from the package page).
```
dotnet new install UltraModding.Templates
```
This will install the following template:

| Templates                         | Short Name      | Language | Tags                                 |
|-----------------------------------|-----------------|----------|--------------------------------------|
| Ultrakill Full Plugin Template    | `ukplugin-full` | `[C#]`   | `BepInEx/BepInEx 5/Plugin/Ultrakill` |
| Ultrakill Minimal Plugin Template | `ukplugin-min`  | `[C#]`   | `BepInEx/BepInEx 5/Plugin/Ultrakill` |

## Using a template

To use a template, you can use the `dotnet new` command.  
If you use Rider or Visual Studio 2022, you will be able to select the templates when creating a new solution.

For example, to create an Ultrakill plugin project:
```bash
dotnet new ukplugin-min -o MyPluginName
```

This will create a folder named MyPluginName with an example plugin project, and with some dependencies.

All templates have additional options. To view them, use `-h` or `--help` switch. For example:

```bash
dotnet new ukplugin-min --help
```

will show additional options you can specify when creating a project:

```text
Options:
  -N|--Name                 The name of the plugin
                            text - Optional
                            Default: My first plugin

  -G|--GUID                 Unique plugin GUID
                            text - Optional
                            Default: com.yourname.pluginname

  -V|--Version              Plugin version
                            text - Optional
                            Default: 1.0.0

  -U|--UltrakillDirectory   Path to Ultrakill directory (If left blank, will use a provided
                            stripped assembly)
                            text - Optional
```

## Documentation, guides and more

For more guides, refer to [BepInEx Docs](https://docs.bepinex.dev) and the (WIP) [UltraModding Wiki](https://ultramodding.github.io/).  
If you're writing a BepInEx plugin for the first time, [check out the plugin development walkthrough](https://docs.bepinex.dev/articles/dev_guide/plugin_tutorial/index.html).
