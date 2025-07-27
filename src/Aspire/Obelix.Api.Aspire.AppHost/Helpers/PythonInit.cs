using System.Diagnostics;

namespace Obelix.Api.Aspire.AppHost.Helpers;

public class PythonInit
{
    public static async Task SetupPythonEnvironmentAsync()
    {
        // Path to the Python project directory
        string projectPath = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "../../../../../Services/Obelix.Api.Services.Analyze"));
        string venvPath = Path.Combine(projectPath, ".venv");
        string requirementsPath = Path.Combine(projectPath, "requirements.txt");

        // Check if requirements.txt exists
        if (!File.Exists(requirementsPath))
        {
            throw new FileNotFoundException($"requirements.txt not found at {requirementsPath}");
        }

        // Create virtual environment if it doesn't exist
        if (!Directory.Exists(venvPath))
        {
            await RunProcessAsync("python", $"-m venv {venvPath}", projectPath);
        }

        // Install requirements
        string pipPath = OperatingSystem.IsWindows() ?
            Path.Combine(venvPath, "Scripts", "pip") :
            Path.Combine(venvPath, "bin", "pip");

        await RunProcessAsync(pipPath, $"install -r requirements.txt", projectPath);
    }

    public static async Task RunProcessAsync(string fileName, string arguments, string workingDirectory)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(startInfo);
        if (process == null)
        {
            throw new Exception($"Failed to start process: {fileName} {arguments}");
        }

        var outputTask = process.StandardOutput.ReadToEndAsync();
        var errorTask = process.StandardError.ReadToEndAsync();

        await process.WaitForExitAsync();
        string output = await outputTask;
        string error = await errorTask;

        if (process.ExitCode != 0)
        {
            throw new Exception($"Process failed with exit code {process.ExitCode}: {error}");
        }

        Console.WriteLine($"Command '{fileName} {arguments}' completed successfully");
        Console.WriteLine(output);
    }
}