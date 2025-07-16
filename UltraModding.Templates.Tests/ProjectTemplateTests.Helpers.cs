using System.Diagnostics;
using System.IO;
using System.Linq;
using UltraModding.Templates.Tests.Helpers;

namespace UltraModding.Templates.Tests;

public sealed partial class ProjectTemplateTests
{
    private bool _isTemplatePacked = false;
    private bool _isTemplateInstalled = false;
    private const string templateProjectDir = @"..\..\..\..\UltraModding.Templates";
    private readonly TempDir _tempPackDir = new() {Output = output};
    private void PackProjectTemplates()
    {
        if (_isTemplatePacked) return;

        var packProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"pack -c Release -o \"{_tempPackDir}\"",
            WorkingDirectory = templateProjectDir,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });
        Assert.NotNull(packProcess);
        packProcess.WaitForExit();
        _isTemplatePacked = true;
    }

    private void InstallProjectTemplate()
    {
        if (_isTemplateInstalled) return;
        PackProjectTemplates();
        var nupkgPath = Directory.GetFiles(_tempPackDir, "*.nupkg").First();
        Assert.NotNull(nupkgPath);
        if (!File.Exists(nupkgPath)) Assert.Fail("Template nupkg not found");
        var installProcess = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"new install \"{nupkgPath}\" {_customHiveArg}",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });
        Assert.NotNull(installProcess);
        installProcess.WaitForExit();
        if (installProcess.ExitCode != 0)
        {
            var errorOutput = installProcess.StandardError.ReadToEnd();
            Assert.Fail($"Error installing template: {errorOutput}");
            return;
        }

        Output.WriteLine("Template installed successfully");
        _isTemplateInstalled = true;
    }

    private void GenerateProjectFromTemplate(
        string workingDirectory,
        string templateName,
        string outputName,
        params string[] args)
    {
        InstallProjectTemplate();
        var process = Process.Start(new ProcessStartInfo
        {
            FileName = "dotnet",
            Arguments = $"new {templateName} {_customHiveArg} -o {outputName} " + string.Join(' ', args),
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false
        });
        Assert.NotNull(process);
        process.WaitForExit();
        if (process.ExitCode != 0)
        {
            var standardOutput = process.StandardOutput.ReadToEnd();
            var errorOutput = process.StandardError.ReadToEnd();
            Assert.Fail($"Error generating template: {standardOutput}\n{errorOutput}");
            return;
        }
        Output.WriteLine("Project generated successfully");
    }

    public void Dispose()
    {
        _tempPackDir.Dispose();
        _hivePath.Dispose();
    }
}