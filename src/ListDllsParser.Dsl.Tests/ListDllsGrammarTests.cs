using System;
using System.Linq;
using NUnit.Framework;
using Sprache;

namespace ListDllsParser.Dsl.Tests
{
    [TestFixture]
    public class ListDllsGrammarTests
    {
        [Test]
        public void HexNumberTest()
        {
            var input = "0x000000002a830000";
            var number = ListDllsGrammar.HexValue.Parse(input);

            Assert.AreEqual(713228288, number);
        }

        [Test]
        public void IdentifierTest()
        {
            var input = " Some: ";
            var output = ListDllsGrammar.Identifier.Parse(input);

            Assert.AreEqual("Some", output);
        }

        [Test]
        public void ProcessInfoNt5ParseAllTest()
        {
            var input = TestInputResources.ProcessInfoNt5;
            var processInfo = ListDllsGrammar.ProcessInfoCollectionNt5.Parse(input).ToList();
            Assert.AreEqual(2, processInfo.Count);
            Assert.AreEqual(3, processInfo[0].Modules.Count());
            Assert.AreEqual(2, processInfo[1].Modules.Count());

            input = TestInputResources.RealListNt5;
            processInfo = ListDllsGrammar.ProcessInfoCollectionNt5.Parse(input).ToList();
            Assert.AreEqual(1, processInfo.Count);
            Assert.AreEqual(49, processInfo[0].Modules.Count());

            var heurap =
                processInfo.First()
                    .Modules.First(m => m.FilePath.EndsWith("heurap.dll"));
            Assert.AreEqual(heurap.Version, "2.2.6.3");
        }

        [Test]
        public void ProcessInfoNt5ParseTest()
        {
            var input = TestInputResources.ProcessInfoNt5;
            var processInfo = ListDllsGrammar.ProcessInfoNt5.Parse(input);

            Assert.AreEqual("cmd.exe", processInfo.Name);
            Assert.AreEqual(188, processInfo.Pid);
            Assert.AreEqual("\"C:\\WINDOWS\\system32\\cmd.exe\"", processInfo.CommandLine);
        }

        [Test]
        public void ProcessInfoParseAllEmptyTest()
        {
            var input = TestInputResources.ProcessInfoEmpty;
            var parser = new ListDllsParser();

            var processInfo = parser.Parse(input).ToList();

            Assert.IsEmpty(processInfo);
        }

        [Test]
        public void ProcessInfoParseAllTest()
        {
            var input = TestInputResources.ProcessInfo;
            var processInfo = ListDllsGrammar.ProcessInfoCollection.Parse(input).ToList();
            Assert.AreEqual(2, processInfo.Count);
            Assert.AreEqual(3, processInfo[0].Modules.Count());
            Assert.AreEqual(2, processInfo[1].Modules.Count());

            input = TestInputResources.RealListNt6;
            processInfo = ListDllsGrammar.ProcessInfoCollection.Parse(input).ToList();
            Assert.AreEqual(1, processInfo.Count);
        }

        [Test]
        public void ProcessInfoParseTest()
        {
            var input = TestInputResources.ProcessInfo;
            var processInfo = ListDllsGrammar.ProcessInfo.Parse(input);

            Assert.AreEqual("cmd.exe", processInfo.Name);
            Assert.AreEqual(3828, processInfo.Pid);
            Assert.AreEqual("\"cmd.exe\" /s /k pushd \"D:\\downloads\"", processInfo.CommandLine);
        }

        [Test]
        public void ProcessModuleInfoNt5ParseTest()
        {
            var input = TestInputResources.ProcessModuleInfoNt5;
            var module = ListDllsGrammar.ProcessModuleInfoNt5.Parse(input);

            Assert.AreEqual(@"C:\WINDOWS\system32\ntdll.dll", module.FilePath);
            Assert.AreEqual(new IntPtr(2089811968), module.Base);
            Assert.AreEqual(729088, module.Size);
            Assert.AreEqual("5.1.2600.6055", module.Version);
        }

        [Test]
        public void ProcessModuleInfoParseTest()
        {
            var input = TestInputResources.ProcessModuleInfo;
            var module = ListDllsGrammar.ProcessModuleInfo.Parse(input);

            Assert.AreEqual(@"C:\Windows\system32\cmd.exe", module.FilePath);
            Assert.AreEqual(new IntPtr(713228288), module.Base);
            Assert.AreEqual(364544, module.Size);
            Assert.AreEqual("Microsoft Windows", module.Verified);
            Assert.AreEqual("Microsoft Corporation", module.Publisher);
            Assert.AreEqual("", module.Description);
            Assert.AreEqual("Операционная система Microsoft® Windows®", module.Product);
            Assert.AreEqual("6.2.14393.0", module.FileVersion);
            Assert.AreEqual(new DateTime(2016, 7, 16, 5, 23, 21), module.CreateTime);
        }

        [Test]
        public void PropertyNameTest()
        {
            var input = " Some property: ";
            var output = ListDllsGrammar.PropertyName.Parse(input);
            Assert.AreEqual("Some property", output);

            input = "Property: ";
            output = ListDllsGrammar.PropertyName.Parse(input);
            Assert.AreEqual("Property", output);
        }

        [Test]
        public void PropertyTest()
        {
            var input = "Some property: some value" + Environment.NewLine;
            var value = ListDllsGrammar.Property.Parse(input);

            Assert.AreEqual("some value", value);
        }

        [Test]
        public void PropertyValueTest()
        {
            var input = "Some value" + Environment.NewLine + "something";
            var output = ListDllsGrammar.PropertyValue.Parse(input);

            Assert.AreEqual("Some value", output);
        }

        [Test]
        public void PropertyWithoutValueTest()
        {
            var input = "Some property:\t" + Environment.NewLine;
            var value = ListDllsGrammar.Property.Parse(input);

            Assert.AreEqual("", value);
        }
    }
}