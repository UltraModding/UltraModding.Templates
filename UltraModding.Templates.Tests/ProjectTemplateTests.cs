using System.Diagnostics;
using System.Xml;
using Xunit.Abstractions;

namespace UltraModding.Templates.Tests;

public sealed partial class ProjectTemplateTests(ITestOutputHelper output) : IDisposable
{
    private readonly TempDir _hivePath = new("dotnet-hive-" + Guid.NewGuid()) {Output = output};
    private string _customHiveArg => $"--debug:custom-hive \"{_hivePath}\"";
    private ITestOutputHelper Output => output;
    public static TheoryData<string> TemplateNames => ["ukplugin-min"];

    [Theory]
    [MemberData(nameof(TemplateNames))]
    public void Template_Generates_Project_Successfully(string templateName)
    {
        using TempDir tempDir = new();
        
        GenerateProjectFromTemplate(tempDir, templateName, "GeneratedProject");

        var projectPath = Path.Combine(tempDir, "GeneratedProject", "GeneratedProject.csproj");
        Assert.True(File.Exists(projectPath), "Project file was not created.");
    }

    [Theory] // Maybe this should be a Fact once other templates exist. Or refactor if necessary
    [MemberData(nameof(TemplateNames))]
    public void Template_Replaces_Parameters_In_Csproj(string templateName)
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
}