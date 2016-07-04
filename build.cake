#addin "Cake.Git"
#addin "Cake.Powershell"
#tool "nuget:?package=xunit.runner.console"
#r "System.Net.Http"

using System.Diagnostics;
using System.Net.Http;
using System.Net.Http.Headers;

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var nugetSource = Argument("nugetSource", "https://www.nuget.org/api/v2/package");
var nugetApiKey = Argument("nugetApiKey", "");
var githubApiKey = Argument("githubApiKey", "");

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
		var headSha = GitLogTip(Directory(".")).Sha;
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
			OutputDirectory = "./build",
		});
	});

Task("NuGetPublishOnly")
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

Task("NuGetTagOnly")
	.IsDependentOn("NuGetPack")
	.WithCriteria(() => !string.IsNullOrEmpty(githubApiKey))
	.Does(() =>
	{
		var version = GetSemVerFromFile(assemblyPath);
		var headSha = GitLogTip(Directory(".")).Sha;
		var tagName = $"nuget-{version}";
		Information($"Creating git tag '{tagName}'...");
		var httpClient = new HttpClient();
		httpClient.DefaultRequestHeaders.Add("User-Agent", "build.cake");
		var httpRequest = new HttpRequestMessage(HttpMethod.Post, "https://api.github.com/repos/Faithlife/Parsing/git/refs");
		httpRequest.Headers.Authorization = AuthenticationHeaderValue.Parse($"token {githubApiKey}");
		httpRequest.Content = new StringContent($"{{\"ref\":\"refs/tags/{tagName}\",\"sha\":\"{headSha}\"}}", Encoding.UTF8, "application/json");
		var httpResponse = httpClient.SendAsync(httpRequest).GetAwaiter().GetResult();
		if (!httpResponse.IsSuccessStatusCode)
			throw new InvalidOperationException($"GitHub tag creation failed with {httpResponse.StatusCode}: {httpResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult()}");
	});

Task("NuGetPublish")
	.IsDependentOn("NuGetPublishOnly")
	.IsDependentOn("NuGetTagOnly");

Task("Default")
	.IsDependentOn("Test");

RunTarget(target);
