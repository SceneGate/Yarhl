namespace BuildSystem;

using System.Xml.Linq;
using Cake.Frosting;
using Cake.Frosting.PleOps.Recipe;

[TaskName("Prepare-IntegrationTests")]
[TaskDescription("Prepare the integration tests by copying an example of DLL")]
[IsDependeeOf(typeof(Cake.Frosting.PleOps.Recipe.Dotnet.TestTask))]
public class PrepareIntegrationTestsTask : FrostingTask<PleOpsBuildContext>
{
    public override void Run(PleOpsBuildContext context)
    {
        // Copy a good and bad plugin to test the assembly load logic
        string badPluginPath = context.DotNetContext.SolutionPath; // this isn't a DLL for sure :D

        string outputBasePath = $"src/Yarhl.IntegrationTests/bin/{context.DotNetContext.Configuration}";
        string testProjectPath = "src/Yarhl.IntegrationTests/Yarhl.IntegrationTests.csproj";

        foreach (string framework in GetTargetFrameworks(testProjectPath)) {
            string pluginPath = $"src/Yarhl.Media.Text/bin/{context.DotNetContext.Configuration}/{framework}/Yarhl.Media.Text.dll";
            string pluginDir = $"{outputBasePath}/{framework}/Plugins";
            _ = Directory.CreateDirectory(pluginDir);
            File.Copy(pluginPath, $"{pluginDir}/Yarhl.Media.Text.dll", true);
            File.Copy(badPluginPath, $"{pluginDir}/MyBadPlugin.dll", true);
        }
    }

    private static IEnumerable<string> GetTargetFrameworks(string projectPath)
    {
        XElement projectXml = XDocument.Load(projectPath).Root
            ?? throw new Exception("Invalid csproj file");
        var frameworks = projectXml.Elements("PropertyGroup")
            .Where(x => x.Element("TargetFrameworks") != null)
            .SelectMany(x => x.Element("TargetFrameworks")!.Value.Split(';'))
            .ToList();

        string? singleFramework = projectXml.Elements("PropertyGroup")
            .Select(x => x.Element("TargetFramework")?.Value)
            .FirstOrDefault();
        if (singleFramework != null && !frameworks.Contains(singleFramework)) {
            frameworks.Add(singleFramework);
        }

        return frameworks;
    }
}
