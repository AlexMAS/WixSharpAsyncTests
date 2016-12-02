using System;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Runtime.Serialization;
using System.Threading.Tasks;

using NUnit.Framework;

using WixSharp;

namespace WixSharpAsyncTests
{
    [TestFixture]
    public class WixSharpAsyncTest
    {
        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            // $(OutDir)
            _outputPath = Path.GetDirectoryName(new Uri(System.Reflection.Assembly.GetExecutingAssembly().CodeBase).LocalPath);

            // $(SolutionDir)packages\WixSharp.wix.bin.3.10.1\tools\bin
            var wixToolsetPath = Path.Combine(_outputPath, @"..\..\..\packages\WixSharp.wix.bin.3.10.1\tools\bin");

            Compiler.WixLocation = wixToolsetPath;
        }

        [TearDown]
        public void TearDown()
        {
            // Clears context after each test
            CallContext.LogicalSetData("key1", null);
        }


        private string _outputPath;


        [Test]
        public void BuildSync_When_Clean_Context()
        {
            // Given
            var installerPath = Path.Combine(_outputPath, $"{nameof(BuildSync_When_Clean_Context)}.msi");
            var project = TestProjectBuilder.CreateProject(_outputPath);

            // When
            TestDelegate buildMsi = () =>
                Compiler.BuildMsi(project, installerPath);

            // Then
            Assert.DoesNotThrow(buildMsi);
            Assert.True(System.IO.File.Exists(installerPath));
        }

        [Test]
        public void BuildSync_When_Dirty_Context()
        {
            // Given
            var installerPath = Path.Combine(_outputPath, $"{nameof(BuildSync_When_Dirty_Context)}.msi");
            var project = TestProjectBuilder.CreateProject(_outputPath);

            // When

            CallContext.LogicalSetData("key1", new SomeContextClass());

            TestDelegate buildMsi = () =>
                Compiler.BuildMsi(project, installerPath);

            // Then
            Assert.Throws<SerializationException>(buildMsi);
            Assert.False(System.IO.File.Exists(installerPath));
        }


        [Test]
        public void BuildAsync_When_Clean_Context()
        {
            // Given
            var installerPath = Path.Combine(_outputPath, $"{nameof(BuildAsync_When_Clean_Context)}.msi");
            var project = TestProjectBuilder.CreateProject(_outputPath);

            // When
            AsyncTestDelegate buildMsi = async () =>
                await Task.Run(() =>
                    Compiler.BuildMsi(project, installerPath));

            // Then
            Assert.DoesNotThrowAsync(buildMsi);
            Assert.True(System.IO.File.Exists(installerPath));
        }

        [Test]
        public void BuildAsync_When_Dirty_Context()
        {
            // Given
            var installerPath = Path.Combine(_outputPath, $"{nameof(BuildAsync_When_Dirty_Context)}.msi");
            var project = TestProjectBuilder.CreateProject(_outputPath);

            // When

            CallContext.LogicalSetData("key1", new SomeContextClass());

            AsyncTestDelegate buildMsi = async () =>
                await Task.Run(() =>
                    Compiler.BuildMsi(project, installerPath));

            // Then
            Assert.ThrowsAsync<SerializationException>(buildMsi);
            Assert.False(System.IO.File.Exists(installerPath));
        }


        [Test]
        public void BuildSync_In_New_Context_When_Clean_Context()
        {
            // Given
            var installerPath = Path.Combine(_outputPath, $"{nameof(BuildSync_In_New_Context_When_Clean_Context)}.msi");
            var project = TestProjectBuilder.CreateProject(_outputPath);

            // When

            TestDelegate buildMsi = () =>
                WixSharpAsyncHelper.ExecuteInNewContext(() =>
                    Compiler.BuildMsi(project, installerPath)).Wait();

            // Then
            Assert.DoesNotThrow(buildMsi);
            Assert.True(System.IO.File.Exists(installerPath));
        }

        [Test]
        public void BuildSync_In_New_Context_When_Dirty_Context()
        {
            // Given
            var installerPath = Path.Combine(_outputPath, $"{nameof(BuildSync_In_New_Context_When_Dirty_Context)}.msi");
            var project = TestProjectBuilder.CreateProject(_outputPath);

            // When

            CallContext.LogicalSetData("key1", new SomeContextClass());

            TestDelegate buildMsi = () =>
                WixSharpAsyncHelper.ExecuteInNewContext(() =>
                    Compiler.BuildMsi(project, installerPath)).Wait();

            // Then
            Assert.DoesNotThrow(buildMsi);
            Assert.True(System.IO.File.Exists(installerPath));
        }


        [Test]
        public void BuildAsync_In_New_Context_When_Clean_Context()
        {
            // Given
            var installerPath = Path.Combine(_outputPath, $"{nameof(BuildAsync_In_New_Context_When_Clean_Context)}.msi");
            var project = TestProjectBuilder.CreateProject(_outputPath);

            // When

            AsyncTestDelegate buildMsi = async () =>
                await WixSharpAsyncHelper.ExecuteInNewContext(() =>
                    Compiler.BuildMsi(project, installerPath));

            // Then
            Assert.DoesNotThrowAsync(buildMsi);
            Assert.True(System.IO.File.Exists(installerPath));
        }

        [Test]
        public void BuildAsync_In_New_Context_When_Dirty_Context()
        {
            // Given
            var installerPath = Path.Combine(_outputPath, $"{nameof(BuildAsync_In_New_Context_When_Dirty_Context)}.msi");
            var project = TestProjectBuilder.CreateProject(_outputPath);

            // When

            CallContext.LogicalSetData("key1", new SomeContextClass());

            AsyncTestDelegate buildMsi = async () =>
                await WixSharpAsyncHelper.ExecuteInNewContext(() =>
                    Compiler.BuildMsi(project, installerPath));

            // Then
            Assert.DoesNotThrowAsync(buildMsi);
            Assert.True(System.IO.File.Exists(installerPath));
        }
    }


    [Serializable]
    public class SomeContextClass
    {
        public object NonSerializedProperty
        {
            get { return new object(); }
            set { throw new Exception(); }
        }
    }
}