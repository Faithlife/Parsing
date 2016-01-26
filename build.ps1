properties {
  $configuration = "Release"
  $gitPath = "git.exe"
  $outputDir = "build"
  $apiKey = $null
  $nugetPackageSource = $null
}

$version = $null

Task Default -depends Test

Task Clean {
  Get-ChildItem "src\*\bin" | Remove-Item -force -recurse -ErrorAction Stop
  Get-ChildItem "src\*\obj" | Remove-Item -force -recurse -ErrorAction Stop
  Get-ChildItem "tests\*\bin" | Remove-Item -force -recurse -ErrorAction Stop
  Get-ChildItem "tests\*\obj" | Remove-Item -force -recurse -ErrorAction Stop
  if (Test-Path $outputDir) {
    Remove-Item $outputDir -force -recurse -ErrorAction Stop
  }
}

Task Build -depends Clean {
  Exec { tools\NuGet\NuGet restore }
  Exec { msbuild /m:4 /p:Configuration=$configuration /p:Platform="Any CPU" /p:VisualStudioVersion=12.0 Parsing.sln }
}

Task Test -depends Build {
  md "build\tests"
  Copy "tests\Faithlife.Parsing.Tests\bin\$configuration\*.dll" "build\tests"
  Copy "src\Faithlife.Parsing\bin\$configuration\Faithlife.Parsing.dll" "build\tests"
  Exec { packages\xunit.runner.console.2.0.0\tools\xunit.console.exe "build\tests\Faithlife.Parsing.Tests.dll" -xml "build\testresults.xml" }
}

Task SourceIndex -depends Test {
  $headSha = & $gitPath rev-parse HEAD
  Exec { tools\SourceIndex\github-sourceindexer.ps1 -symbolsFolder src\Faithlife.Parsing\bin\$configuration -userId Faithlife -repository Parsing -branch $headSha -sourcesRoot ${pwd} -dbgToolsPath "C:\Program Files (x86)\Windows Kits\8.1\Debuggers\x86" -gitHubUrl "https://raw.github.com" -serverIsRaw -ignoreUnknown -verbose }
}

Task NuGetPack -depends SourceIndex {
  mkdir $outputDir -force
  $filePath = Resolve-Path "src\Faithlife.Parsing\bin\$configuration\Faithlife.Parsing.dll"
  $fileVersionInfo = [System.Diagnostics.FileVersionInfo]::GetVersionInfo($filePath)
  $script:version = "$($fileVersionInfo.FileMajorPart).$($fileVersionInfo.FileMinorPart).$($fileVersionInfo.FileBuildPart)"
  Exec { tools\NuGet\NuGet pack Faithlife.Parsing.nuspec -Version $script:version -Prop Configuration=$configuration -Symbols -OutputDirectory $outputDir }
}

Task NuGetPublish -depends NuGetPack -precondition { return $apiKey -and $nugetPackageSource } {
  Exec { tools\NuGet\NuGet push $outputDir\Faithlife.Parsing.$script:version.nupkg -ApiKey $apiKey -Source $nugetPackageSource }
}
