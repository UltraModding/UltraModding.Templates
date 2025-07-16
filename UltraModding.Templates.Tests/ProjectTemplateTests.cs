using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml;
using UltraModding.Templates.Tests.Helpers;

namespace UltraModding.Templates.Tests;

public sealed partial class ProjectTemplateTests(ITestOutputHelper output) : IDisposable
{
    private readonly TempDir _hivePath = new("dotnet-hive-" + Guid.NewGuid()) {Output = output};
    private string _customHiveArg => $"--debug:custom-hive \"{_hivePath}\"";
    private ITestOutputHelper Output => output;
    public static TheoryData<string> TemplateNames => ["ukplugin-min", "ukplugin-full"];

    [Theory]
    [MemberData(nameof(TemplateNames))]
    public void Template_Generates_Project_Successfully(string templateName)
    {
        using TempDir tempDir = new();
        
        GenerateProjectFromTemplate(tempDir, templateName, "GeneratedProject");

        var projectPath = Path.Combine(tempDir, "GeneratedProject", "GeneratedProject.csproj");
        Assert.True(File.Exists(projectPath), "Project file was not created.");
    }

    [Theory]
    [MemberData(nameof(TemplateNames))]
    public void Template_Replaces_Common_Parameters_In_Csproj(string templateName)
    {
        using TempDir tempDir = new();
        
        const string pluginName = "TestPlugin";
        const string assemblyName = "TestPluginAssembly";
        const string guid = "com.example.testplugin";
        const string version = "2.3.4";

        GenerateProjectFromTemplate(
            workingDirectory: tempDir,
            templateName: templateName,
            outputName: assemblyName,
            $"--Name \"{pluginName}\"",
            $"--GUID \"{guid}\"", 
            $"--Version \"{version}\"");

        var projectDir = Path.Combine(tempDir, assemblyName);
        var csprojFiles = Directory.GetFiles(projectDir, "*.csproj");
        Assert.True(csprojFiles.Length == 1, "No .csproj file found.");

        Output.WriteLine("Generated .csproj content");

        var xmlDoc = new XmlDocument();
        xmlDoc.Load(csprojFiles[0]);
        var root = xmlDoc.DocumentElement;
        Assert.NotNull(root);

        // Check if <AssemblyName>TestPluginAssembly</AssemblyName> exists (-o <assemblyName>)
        var assemblyNameNode = xmlDoc.SelectSingleNode("//AssemblyName");
        Assert.NotNull(assemblyNameNode);
        Assert.Equal(assemblyName, assemblyNameNode.InnerText);

        // Check if <Product>TestPlugin</Product> exists (--Name <pluginName>)
        var productNode = xmlDoc.SelectSingleNode("//Product");
        Assert.NotNull(productNode);
        Assert.Equal(pluginName, productNode.InnerText);

        // Check if <Version>2.3.4</Version> exists (--Version <version>)
        var versionNode = xmlDoc.SelectSingleNode("//Version");
        Assert.NotNull(versionNode);
        Assert.Equal(version, versionNode.InnerText);

        // Check if <GUID>com.example.testplugin</GUID> exists (--GUID <guid>)
        var guidNode = xmlDoc.SelectSingleNode("//GUID");
        Assert.NotNull(guidNode);
        Assert.Equal(guid, guidNode.InnerText);
    }

    [Fact]
    public void MinimalProjectTemplate_Generated_Project_Builds()
    {
        // So far, only the minimal template is planned to be able to generate and build with a stripped assembly
        const string templateName = "ukplugin-min";
        using TempDir tempDir = new();
        
        const string pluginName = "TestPlugin_BuildTest";
        const string assemblyName = "TestPluginAssembly_BuildTest";
        const string guid = "com.example.testpluginbuildtest";
        const string version = "2.3.4";

        GenerateProjectFromTemplate(
            workingDirectory: tempDir,
            templateName: templateName, 
            outputName: assemblyName,
            $"--Name \"{pluginName}\"",
            $"--GUID \"{guid}\"",
            $"--Version \"{version}\"");
        
        var buildProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = "build",
            WorkingDirectory = Path.Combine(tempDir, assemblyName),
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });
        Assert.NotNull(buildProcess);
        buildProcess.WaitForExit();
        if (buildProcess.ExitCode != 0)
        {
            var errorOutput = buildProcess.StandardOutput.ReadToEnd();
            Assert.Fail($"Error building project: {errorOutput}");
            return;
        }
        Output.WriteLine("Project built successfully");
    }

    [Fact]
    public async Task FullProjectTemplate_Generated_Project_Builds()
    {
        const string templateName = "ukplugin-full";
        const string pluginName = "TestPlugin_BuildTest";
        const string assemblyName = "TestPluginAssembly_BuildTest";
        const string guid = "com.example.testpluginbuildtest";
        const string version = "2.3.4";
        const string ultrakillDirectory = "C:/Program Files (x86)/Steam/steamapps/common/ULTRAKILL/";
        const string pluginsDirectory = $"{ultrakillDirectory}BepInEx/plugins/";
        
        Assert.SkipUnless(Directory.Exists(pluginsDirectory),
            $"Ultrakill plugins directory '{pluginsDirectory}' does not exist. Skipping test.");
        
        var cancellationToken = TestContext.Current.CancellationToken;
        using TempDir tempDir = new();
        
        GenerateProjectFromTemplate(
            workingDirectory: tempDir,
            templateName: templateName,
            outputName: assemblyName,
            $"--Name \"{pluginName}\"",
            $"--GUID \"{guid}\"",
            $"--Version \"{version}\"",
            $"--UltrakillDirectory \"{ultrakillDirectory}\"");

        var buildProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "build -c Release",
                WorkingDirectory = Path.Combine(tempDir, assemblyName),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            }
        };
        Assert.NotNull(buildProcess);
        #if DEBUG
        buildProcess.OutputDataReceived += (_, args) 
            => Output.WriteLine($"StdOut: {args.Data}");
        buildProcess.ErrorDataReceived += (_, args)
            => Output.WriteLine($"StdErr: {args.Data}");
        #endif
        buildProcess.Start();
        buildProcess.BeginOutputReadLine();
        buildProcess.BeginErrorReadLine();
        
        
        await buildProcess.WaitForExitAsync(cancellationToken);
        
        if (buildProcess.ExitCode != 0)
        {
            var errorTask = buildProcess.StandardError.ReadToEndAsync(cancellationToken);
            Assert.Fail($"Error building project: {await errorTask}");
            return;
        }
        Output.WriteLine("Project built successfully");
    }
    
}