using WixSharp;

namespace WixSharpAsyncTests
{
    public class TestProjectBuilder
    {
        public static ManagedProject CreateProject(string outputPath)
        {
            const string projectName = "FakeInstaller";
            var projectSourceFile = typeof(TestProjectBuilder).Assembly.Location;
            var projectDir = new Dir(projectName, new Files(projectSourceFile));
            var project = new ManagedProject(projectName, projectDir);
            project.OutDir = outputPath;
            return project;
        }
    }
}