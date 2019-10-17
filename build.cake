//
//  build.cake
//
//  Author:
//       Benito Palacios Sánchez (aka pleonex) <benito356@gmail.com>
//
//  Copyright (c) 2018 Benito Palacios Sánchez
//
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
//
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU General Public License for more details.
//
//  You should have received a copy of the GNU General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

// NUnit tests
#tool nuget:?package=NUnit.ConsoleRunner&version=3.10.0

// Gendarme: decompress zip
#addin nuget:?package=Cake.Compression&loaddependencies=true&version=0.2.3

// Test coverage
#addin nuget:?package=altcover.api&version=6.0.700
#tool nuget:?package=ReportGenerator&version=4.2.15

// Documentation
#addin nuget:?package=Cake.DocFx&version=0.13.0
#tool nuget:?package=docfx.console&version=2.44.0

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var tests = Argument("tests", string.Empty);
var warnAsError = Argument("warnaserror", false);
var warnAsErrorOption = warnAsError
    ? MSBuildTreatAllWarningsAs.Error
    : MSBuildTreatAllWarningsAs.Default;

var pullRequestNumber = Argument("pr-number", string.Empty);
var pullRequestBase = Argument("pr-base", string.Empty);
var pullRequestBranch = Argument("pr-branch", string.Empty);
var branchName = Argument("branch", string.Empty);

string netVersion = "48";
string netcoreVersion = "3.0";
string netstandardVersion = "2.0";

string solutionPath = "src/Yarhl.sln";

string netBinDir = $"bin/{configuration}/net{netVersion}";
string netcoreBinDir = $"bin/{configuration}/netcoreapp{netcoreVersion}";
string netstandardBinDir = $"bin/{configuration}/netstandard{netstandardVersion}";

Task("Clean")
    .Does(() =>
{
    DotNetCoreClean(solutionPath, new DotNetCoreCleanSettings {
        Configuration = "Debug",
        Verbosity = DotNetCoreVerbosity.Minimal,
    });
    DotNetCoreClean(solutionPath, new DotNetCoreCleanSettings {
        Configuration = "Release",
        Verbosity = DotNetCoreVerbosity.Minimal,
    });

    if (DirectoryExists("artifacts")) {
        DeleteDirectory(
            "artifacts",
            new DeleteDirectorySettings { Recursive = true });
    }
});

Task("Build")
    .Does(() =>
{
    DotNetCoreBuild(solutionPath, new DotNetCoreBuildSettings {
        Configuration = configuration,
        Verbosity = DotNetCoreVerbosity.Minimal,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .TreatAllWarningsAs(warnAsErrorOption),
    });

    // Copy Yarhl.Media for the integration tests
    EnsureDirectoryExists($"src/Yarhl.IntegrationTests/{netBinDir}/Plugins");
    CopyFileToDirectory(
        $"src/Yarhl.Media/{netstandardBinDir}/Yarhl.Media.dll",
        $"src/Yarhl.IntegrationTests/{netBinDir}/Plugins");

    EnsureDirectoryExists($"src/Yarhl.IntegrationTests/{netcoreBinDir}/Plugins");
    CopyFileToDirectory(
        $"src/Yarhl.Media/{netstandardBinDir}/Yarhl.Media.dll",
        $"src/Yarhl.IntegrationTests/{netcoreBinDir}/Plugins");
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    // NUnit3 to test libraries with .NET Framework / Mono
    var settings = new NUnit3Settings();
    settings.NoResults = false;
    if (tests != string.Empty) {
        settings.Test = tests;
    }

    var testAssemblies = new List<FilePath> {
        $"src/Yarhl.UnitTests/{netBinDir}/Yarhl.UnitTests.dll",
        $"src/Yarhl.IntegrationTests/{netBinDir}/Yarhl.IntegrationTests.dll"
    };
    NUnit3(testAssemblies, settings);

    // .NET Core test library
    var netcoreSettings = new DotNetCoreTestSettings {
        NoBuild = true,
        Framework = $"netcoreapp{netcoreVersion}"
    };

    if (tests != string.Empty) {
        netcoreSettings.Filter = $"FullyQualifiedName~{tests}";
    }

    DotNetCoreTest(
        $"src/Yarhl.UnitTests/Yarhl.UnitTests.csproj",
        netcoreSettings);
    DotNetCoreTest(
        $"src/Yarhl.IntegrationTests/Yarhl.IntegrationTests.csproj",
        netcoreSettings);
});

Task("Run-Linter-Gendarme")
    .IsDependentOn("Build")
    .Does(() =>
{
    if (IsRunningOnWindows()) {
        throw new Exception("Gendarme is not supported on Windows");
    }

    var monoTools = DownloadFile("https://github.com/pleonex/mono-tools/releases/download/v4.2.2/mono-tools-v4.2.2.zip");
    ZipUncompress(monoTools, "tools/mono_tools");
    var gendarme = "tools/mono_tools/bin/gendarme";
    if (StartProcess("chmod", $"+x {gendarme}") != 0) {
        Error("Cannot change gendarme permissions");
    }

    RunGendarme(
        gendarme,
        $"src/Yarhl/{netstandardBinDir}/Yarhl.dll",
        "src/Yarhl/Gendarme.ignore");
    RunGendarme(
        gendarme,
        $"src/Yarhl.Media/{netstandardBinDir}/Yarhl.Media.dll",
        "src/Yarhl.Media/Gendarme.ignore");
});

public void RunGendarme(string gendarme, string assembly, string ignore)
{
    var retcode = StartProcess(gendarme, $"--ignore {ignore} {assembly}");
    if (retcode != 0) {
        ReportWarning($"Gendarme found errors on {assembly}");
    }
}

Task("Run-AltCover")
    .IsDependentOn("Build")
    .Does(() =>
{
    // Configure the tests to run with code coverate
    TestWithAltCover(
        "src/Yarhl.UnitTests",
        "Yarhl.UnitTests.dll",
        "coverage_unit.xml");

    TestWithAltCover(
        "src/Yarhl.IntegrationTests",
        "Yarhl.IntegrationTests.dll",
        "coverage_integration.xml");

    // Create the report
    ReportGenerator(
        new FilePath[] { "coverage_unit.xml", "coverage_integration.xml" },
        "coverage_report",
        new ReportGeneratorSettings {
            ReportTypes = new[] {
                ReportGeneratorReportType.Cobertura,
                ReportGeneratorReportType.HtmlInline_AzurePipelines } });

    // Get final result
    var xml = System.Xml.Linq.XDocument.Load("coverage_report/Cobertura.xml");
    var lineRate = xml.Root.Attribute("line-rate").Value;
    if (lineRate == "1") {
        Information("Full coverage!");
    } else {
        ReportWarning($"Missing coverage: {lineRate}");
    }
});

public void TestWithAltCover(string projectPath, string assembly, string outputXml)
{
    string inputDir = $"{projectPath}/{netBinDir}";
    string outputDir = $"{inputDir}/__Instrumented";
    if (DirectoryExists(outputDir)) {
        DeleteDirectory(
            outputDir,
            new DeleteDirectorySettings { Recursive = true });
    }

    var altcoverArgs = new AltCover.Parameters.Primitive.PrepareArgs {
        InputDirectories = new[] { inputDir },
        OutputDirectories = new[] { outputDir },
        AssemblyFilter = new[] { "nunit.framework", "NUnit3" },
        AttributeFilter = new[] { "ExcludeFromCodeCoverage" },
        TypeFilter = new[] { "Yarhl.AssemblyUtils" },
        XmlReport = outputXml,
        OpenCover = true
    };
    Prepare(altcoverArgs);

    string pluginDir = $"{inputDir}/Plugins";
    if (DirectoryExists(pluginDir)) {
        EnsureDirectoryExists($"{outputDir}/Plugins");
        CopyDirectory(pluginDir, $"{outputDir}/Plugins");
    }

    NUnit3($"{outputDir}/{assembly}", new NUnit3Settings { NoResults = true });
}

Task("Build-Doc")
    .IsDependentOn("Build")
    .Does(() =>
{
    // Workaround for
    // https://github.com/dotnet/docfx/issues/3389
    NuGetInstall("SQLitePCLRaw.core", new NuGetInstallSettings {
        Version = "1.1.14",
        ExcludeVersion  = true,
        OutputDirectory = "./tools"
    });
    CopyFileToDirectory(
        "tools/SQLitePCLRaw.core/lib/net45/SQLitePCLRaw.core.dll",
        GetDirectories("tools/docfx.console.*").Single().Combine("tools"));

    DocFxMetadata("docs/docfx.json");
    DocFxBuild("docs/docfx.json");
});

Task("Serve-Doc")
    .IsDependentOn("Build-Doc")
    .Does(() =>
{
    DocFxBuild("docs/docfx.json", new DocFxBuildSettings { Serve = true });
});

Task("Deploy-Doc")
    .IsDependentOn("Build-Doc")
    .Does(() =>
{
    int retcode;

    // Clone or pull
    var repo_doc = Directory("doc-branch");
    if (!DirectoryExists(repo_doc)) {
        retcode = StartProcess(
            "git",
            $"clone git@github.com:SceneGate/Yarhl.git {repo_doc} -b gh-pages");
        if (retcode != 0) {
            throw new Exception("Cannot clone repository");
        }
    } else {
        retcode = StartProcess("git", new ProcessSettings {
            Arguments = "pull",
            WorkingDirectory = repo_doc
        });
        if (retcode != 0) {
            throw new Exception("Cannot pull repository");
        }
    }

    // Copy the content of the web
    CopyDirectory("docs/_site", repo_doc);

    // Commit and push
    retcode = StartProcess("git", new ProcessSettings {
        Arguments = "add --all",
        WorkingDirectory = repo_doc
    });
    if (retcode != 0) {
        throw new Exception("Cannot add files");
    }

    retcode = StartProcess("git", new ProcessSettings {
        Arguments = "commit -m \"Update doc from Cake\"",
        WorkingDirectory = repo_doc
    });
    if (retcode != 0) {
        throw new Exception("Cannot commit");
    }

    retcode = StartProcess("git", new ProcessSettings {
        Arguments = "push origin gh-pages",
        WorkingDirectory = repo_doc
    });
    if (retcode != 0) {
        throw new Exception("Cannot push");
    }
});

Task("Pack")
    .Description("Create the NuGet package")
    .Does(() =>
{
    var settings = new DotNetCorePackSettings {
        Configuration = "Release",
        OutputDirectory = "artifacts/",
        IncludeSymbols = true,
        MSBuildSettings = new DotNetCoreMSBuildSettings()
            .TreatAllWarningsAs(warnAsErrorOption)
            .WithProperty("SymbolPackageFormat", "snupkg")
    };
    DotNetCorePack("src/Yarhl.sln", settings);
});

Task("Deploy")
    .Description("Deploy the NuGet packages to the server")
    .IsDependentOn("Clean")
    .IsDependentOn("Pack")
    .Does(() =>
{
    var settings = new DotNetCoreNuGetPushSettings {
        Source = "https://api.nuget.org/v3/index.json",
        ApiKey = Environment.GetEnvironmentVariable("NUGET_KEY"),
    };
    DotNetCoreNuGetPush(System.IO.Path.Combine("artifacts", "*.nupkg"), settings);
});

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Run-AltCover");

Task("CI-Linux")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Run-Linter-Gendarme")
    .IsDependentOn("Run-AltCover")
    //.IsDependentOn("Build-Doc")  // Waiting for https://github.com/dotnet/docfx/issues/4857
    .IsDependentOn("Pack");

Task("CI-MacOS")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Run-AltCover");

Task("CI-Windows")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Run-AltCover");

RunTarget(target);


public void ReportWarning(string msg)
{
    if (warnAsError) {
        throw new Exception(msg);
    } else {
        Warning(msg);
    }
}
