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
#tool "nuget:?package=NUnit.ConsoleRunner"
#addin nuget:?package=SharpZipLib
#addin nuget:?package=Cake.Compression
#addin nuget:?package=Cake.Incubator&version=3.0.0
#addin nuget:?package=Cake.FileHelpers
#addin nuget:?package=altcover.api
#tool "nuget:?package=ReportGenerator"
#tool coveralls.io
#addin Cake.Coveralls
#addin "nuget:?package=Cake.Sonar"
#tool "nuget:?package=MSBuild.SonarQube.Runner.Tool"
#addin Cake.DocFx
#tool nuget:?package=docfx.console

var netVersion = "472";
var netCoreVersion = "2.1";
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Debug");
var tests = Argument("tests", string.Empty);

var msbuildConfig = new MSBuildSettings {
    Verbosity = Verbosity.Minimal,
    Configuration = configuration,
    Restore = true,
    MaxCpuCount = 0,  // Auto build parallel mode
    WarningsAsError = Argument("warnaserror", false)
};

Task("Build")
    .Does(() =>
{
    MSBuild("src/Yarhl.sln", msbuildConfig);

    // Copy Yarhl.Media for the integration tests
    EnsureDirectoryExists($"src/Yarhl.IntegrationTests/bin/{configuration}/net{netVersion}/Plugins");
    EnsureDirectoryExists($"src/Yarhl.IntegrationTests/bin/{configuration}/netcoreapp{netCoreVersion}/Plugins");
    CopyFileToDirectory(
        $"src/Yarhl.Media/bin/{configuration}/netstandard2.0/Yarhl.Media.dll",
        $"src/Yarhl.IntegrationTests/bin/{configuration}/net{netVersion}/Plugins");
    CopyFileToDirectory(
        $"src/Yarhl.Media/bin/{configuration}/netstandard2.0/Yarhl.Media.dll",
        $"src/Yarhl.IntegrationTests/bin/{configuration}/netcoreapp{netCoreVersion}/Plugins");
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    // NUnit3 to test libraries with .NET Framework / Mono
    var settings = new NUnit3Settings();
    settings.NoResults = true;

    if (tests != string.Empty) {
        settings.Test = tests;
    }

    var testAssemblies = new List<FilePath> {
        $"src/Yarhl.UnitTests/bin/{configuration}/net{netVersion}/Yarhl.UnitTests.dll",
        $"src/Yarhl.IntegrationTests/bin/{configuration}/net{netVersion}/Yarhl.IntegrationTests.dll"
    };
    NUnit3(testAssemblies, settings);

    // .NET Core test library
    var netcoreSettings = new DotNetCoreTestSettings {
        NoBuild = true,
        Framework = $"netcoreapp{netCoreVersion}"
    };
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
    var mono_tools = DownloadFile("https://github.com/pleonex/mono-tools/releases/download/v4.2.2/mono-tools-v4.2.2.zip");
    ZipUncompress(mono_tools, "tools/mono_tools");
    var gendarme = "tools/mono_tools/bin/gendarme";
    if (!IsRunningOnWindows()) {
        if (StartProcess("chmod", $"+x {gendarme}") != 0) {
            Error("Cannot change gendarme permissions");
        }
    }

    RunGendarme(gendarme, "src/Yarhl/Yarhl.csproj", "src/Yarhl/Gendarme.ignore");
    RunGendarme(gendarme, "src/Yarhl.Media/Yarhl.Media.csproj", "src/Yarhl.Media/Gendarme.ignore");
});

public void RunGendarme(string gendarme, string project, string ignore)
{
    var assembly = GetProjectAssemblies(project, configuration).Single();
    var retcode = StartProcess(gendarme, $"--ignore {ignore} {assembly}");
    if (retcode != 0) {
        Warning($"Gendarme found errors on {assembly}");
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
        "coveragereport",
        new ReportGeneratorSettings {
            ReportTypes = new[] {
                ReportGeneratorReportType.Html,
                ReportGeneratorReportType.TextSummary,
                ReportGeneratorReportType.XmlSummary } });

    // Get final result
    var xml = System.Xml.Linq.XDocument.Load("coveragereport/Summary.xml");
    var coverage = xml.Root.Element("Summary").Element("Linecoverage").Value;
    if (coverage == "100%") {
        Information("Full coverage!");
    } else {
        Warning($"Missing coverage: {coverage}");
    }
});

public void TestWithAltCover(string projectPath, string assembly, string outputXml)
{
    string inputDir = $"{projectPath}/bin/{configuration}";
    string outputDir = $"{inputDir}/__Instrumented";
    if (DirectoryExists(outputDir)) {
        DeleteDirectory(
            outputDir,
            new DeleteDirectorySettings { Recursive = true });
    }

    var altcoverArgs = new AltCover.PrepareArgs {
        InputDirectory = inputDir,
        OutputDirectory = outputDir,
        AssemblyFilter = new[] { "nunit.framework" },
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

Task("Test-Quality")
    .IsDependentOn("Run-Linter-Gendarme")
    .IsDependentOn("Run-AltCover");


Task("Run-Coveralls")
    .IsDependentOn("Run-AltCover")
    .Does(() =>
{
    CoverallsIo(
        MakeAbsolute(File("coveragereport/Summary.xml")).FullPath,
        new CoverallsIoSettings {
            RepoToken = EnvironmentVariable("COVERALLS_REPO_TOKEN")
        });
});

Task("Run-Sonar")
    .IsDependentOn("Build")
    .IsDependentOn("Run-AltCover")
    .Does(() =>
{
    var sonar_token = EnvironmentVariable("SONAR_TOKEN");
    SonarBegin(new SonarBeginSettings{
        Url = "https://sonarqube.com",
        Key = "yarhl",
        Login = sonar_token,
        Organization = "pleonex-github",
        Verbose = true
     });

    MSBuild("src/Yarhl.sln", configurator =>
            configurator.SetConfiguration(configuration)
                .WithTarget("Rebuild"));

     SonarEnd(new SonarEndSettings{
        Login = sonar_token
     });
});

Task("Build-Doc")
    .IsDependentOn("Build")
    .Does(() =>
{
    // Workaround for
    // https://github.com/dotnet/docfx/issues/3389
    NuGetInstall("SQLitePCLRaw.core", new NuGetInstallSettings {
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
        Arguments = "commit -a -u -m 'Update doc from cake'",
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

Task("Default")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Test-Quality");

Task("Travis")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Test-Quality")
    .IsDependentOn("Build-Doc");  // Try to build the doc but don't deploy

Task("AppVeyor")
    .IsDependentOn("Build")
    .IsDependentOn("Run-Unit-Tests")
    // Cake doesn't support ReportGenerator with Cobertura
    // and the old Coveralls CLI only support Cobertura.
    // After porting to .NET Core we would be able to use the new CLI.
    // .IsDependentOn("Run-Coveralls")
    .IsDependentOn("Run-Sonar");

RunTarget(target);
