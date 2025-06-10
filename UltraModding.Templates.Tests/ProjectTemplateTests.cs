using System.Diagnostics;
using System.Xml;
using Xunit.Abstractions;

namespace UltraModding.Templates.Tests;

public class ProjectTemplateTests(ITestOutputHelper output)
{
    private readonly ITestOutputHelper _output = output;

    public static TheoryData<string> TemplateNames => ["ukplugin-min"];

    private static void GenerateProjectFromTemplate(string workingDirectory, string args)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = args,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });
        Assert.NotNull(process);
        process.WaitForExit();
    }

    [Theory]
    [MemberData(nameof(TemplateNames))]
    public void Template_Generates_Project_Successfully(string templateName)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        try
        {
            GenerateProjectFromTemplate(tempDir, $"new {templateName} -o GeneratedProject");

            var projectPath = Path.Combine(tempDir, "GeneratedProject", "GeneratedProject.csproj");
            Assert.True(File.Exists(projectPath), "Project file was not created.");
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    [Theory]
    [MemberData(nameof(TemplateNames))]
    public void Template_Replaces_Parameters_In_Csproj(string templateName)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);

        const string pluginName = "TestPlugin";
        const string assemblyName = "TestPluginAssembly";
        const string guid = "com.example.testplugin";
        const string version = "2.3.4";

        try
        {
            GenerateProjectFromTemplate(tempDir, 
                $"new {templateName} " +
                $"--Name \"{pluginName}\" " +
                $"--GUID \"{guid}\" " +
                $"--Version \"{version}\" " +
                $"-o {assemblyName}");

            var projectDir = Path.Combine(tempDir, assemblyName);
            var csprojFiles = Directory.GetFiles(projectDir, "*.csproj");
            Assert.True(csprojFiles.Length == 1, "No .csproj file found.");

            _output.WriteLine("Generated .csproj content");

            var xmlDoc = new XmlDocument();
            xmlDoc.Load(csprojFiles[0]);
            var root = xmlDoc.DocumentElement;
            Assert.NotNull(root);

            // Example: Check if <AssemblyName>TestPluginAssembly</AssemblyName> exists
            var assemblyNameNode = xmlDoc.SelectSingleNode("//AssemblyName");
            Assert.NotNull(assemblyNameNode);
            Assert.Equal(assemblyName, assemblyNameNode.InnerText);

            // Example: Check if <Version>2.3.4</Version> exists
            var versionNode = xmlDoc.SelectSingleNode("//Version");
            Assert.NotNull(versionNode);
            Assert.Equal(version, versionNode.InnerText);

            // Example: Check if <GUID>com.example.testplugin</GUID> exists (if present)
            var guidNode = xmlDoc.SelectSingleNode("//GUID");
            if (guidNode != null)
                Assert.Equal(guid, guidNode.InnerText);
        }
        finally
        {
            Directory.Delete(tempDir, true);
        }
    }

    // [Theory] // TODO: Firstly add the stripped assembly to the template
    // [MemberData(nameof(TemplateNames))]
    public void Generated_Project_Builds(string templateName)
    {
        var tempDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(tempDir);
        try
        {
            GenerateProjectFromTemplate(tempDir, $"new {templateName} -o GeneratedProject");

            var buildProcess = Process.Start(new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = "build",
                WorkingDirectory = Path.Combine(tempDir, "GeneratedProject"),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false
            });
            buildProcess.WaitForExit();
            Assert.Equal(0, buildProcess.ExitCode);

        }
        finally
        {
            Directory.Delete(tempDir, true);
        }

    }
}