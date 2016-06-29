#addin "Cake.Powershell"
#tool "nuget:?package=xunit.runner.console"

using System.Diagnostics;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var gitPath = Argument("gitPath", "git.exe");
var nugetSource = Argument("nugetSource", "https://www.nuget.org/api/v2/package");
var nugetApiKey = Argument("nugetApiKey", "");

var solutionPath = "./Parsing.sln";
var nugetPackageName = "Faithlife.Parsing";
var assemblyPath = $"./src/Faithlife.Parsing/bin/{configuration}/Faithlife.Parsing.dll";
var pdbRootPath = $"./src/Faithlife.Parsing/bin/{configuration}";

string GetSemVerFromFile(string path)
{
	var versionInfo = FileVersionInfo.GetVersionInfo(path);
	return $"{versionInfo.FileMajorPart}.{versionInfo.FileMinorPart}.{versionInfo.FileBuildPart}";
}

Task("Clean")
	.Does(() =>
	{
		CleanDirectories($"./src/**/bin/{configuration}");
		CleanDirectories($"./src/**/obj/{configuration}");
		CleanDirectories($"./tests/**/bin/{configuration}");
		CleanDirectories($"./tests/**/obj/{configuration}");
	});

Task("NuGetRestore")
	.IsDependentOn("Clean")
	.Does(() => NuGetRestore(solutionPath));

Task("Build")
	.IsDependentOn("NuGetRestore")
	.Does(() => MSBuild(solutionPath, settings => settings.SetConfiguration(configuration)));

Task("Test")
	.IsDependentOn("Build")
	.Does(() => XUnit2($"./tests/**/bin/{configuration}/*.Tests.dll"));

Task("SourceIndex")
	.IsDependentOn("Test")
	.WithCriteria(() => configuration == "Release")
	.Does(() =>
	{
		IEnumerable<string> gitOutput;
		var gitSettings = new ProcessSettings { Arguments = "rev-parse HEAD", RedirectStandardOutput = true };
		var gitExitCode = StartProcess(gitPath, gitSettings, out gitOutput);
		if (gitExitCode != 0)
			throw new InvalidOperationException($"Failed to get HEAD SHA from git. (exit code {gitExitCode})");
		var headSha = gitOutput.FirstOrDefault() ?? "";
		if (headSha.Length != 40)
			throw new InvalidOperationException("Failed to get HEAD SHA from git. (got '{headSha}')");

		StartPowershellFile("./tools/SourceIndex/github-sourceindexer.ps1", new PowershellSettings()
			.WithArguments(args =>
			{
				args.AppendQuoted("symbolsFolder", MakeAbsolute(Directory(pdbRootPath)).FullPath)
					.Append("userId", "Faithlife")
					.Append("repository", "Parsing")
					.Append("branch", headSha)
					.AppendQuoted("sourcesRoot", MakeAbsolute(Directory(".")).FullPath)
					.AppendQuoted("dbgToolsPath", @"C:\Program Files (x86)\Windows Kits\8.1\Debuggers\x86")
					.Append("gitHubUrl", "https://raw.github.com")
					.Append("serverIsRaw", "")
					.Append("ignoreUnknown", "")
					.Append("verbose", "");
			}));
	});

Task("NuGetPack")
	.IsDependentOn("SourceIndex")
	.Does(() =>
	{
		var version = GetSemVerFromFile(assemblyPath);

		CreateDirectory("./build");

		NuGetPack($"./{nugetPackageName}.nuspec", new NuGetPackSettings
		{
			Version = version,
			ArgumentCustomization = args => args.Append($"-Prop Configuration={configuration}"),
			Symbols = true,
			OutputDirectory = "./build",
		});
	});

Task("NuGetPublish")
	.IsDependentOn("NuGetPack")
	.WithCriteria(() => !string.IsNullOrEmpty(nugetSource) && !string.IsNullOrEmpty(nugetApiKey))
	.Does(() =>
	{
		var version = GetSemVerFromFile(assemblyPath);

		NuGetPush($"./build/{nugetPackageName}.{version}.nupkg", new NuGetPushSettings
		{
			ApiKey = nugetApiKey,
			Source = nugetSource.Length == 0 ? null : nugetSource,
		});
	});

Task("Default")
	.IsDependentOn("Test");

RunTarget(target);
