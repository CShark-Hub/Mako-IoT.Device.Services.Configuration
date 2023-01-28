using System;
using System.Diagnostics;
using MakoIoT.Device.Services.Configuration.Test.Mocks;
using MakoIoT.Device.Services.Interface;
using nanoFramework.Logging.Debug;
using nanoFramework.TestFramework;

namespace MakoIoT.Device.Services.Configuration.Test
{
    [TestClass]
    public class ConfigurationServiceTest
    {
        [TestMethod]
        public void GetConfigSection_given_NoFile_ConfigClassWithNoDefault_should_throw_ConfigurationException()
        {
            var sut = new ConfigurationService(new MockStorage(), new DebugLogger(nameof(ConfigurationServiceTest)));

            Assert.ThrowsException(typeof(ConfigurationException),
                () => sut.GetConfigSection("", typeof(TestConfigWithNoDefault)));

        }

        [TestMethod]
        public void GetConfigSection_given_file_should_return_deserialized_object()
        {
            var logger = new MockLogger();
            var storage = new MockStorage
            {
                FileName = $"mako-{TestConfigWithNoDefault.SectionName}.cfg".ToLower(),
                Text = "{\"Setting2\":5,\"Setting1\":\"Value1\"}"
            };
            var sut = new ConfigurationService(storage, logger);

            var result = sut.GetConfigSection(TestConfigWithNoDefault.SectionName, typeof(TestConfigWithNoDefault)) as TestConfigWithNoDefault;

            Assert.IsNotNull(result);
            Assert.AreEqual("Value1", result.Setting1);
            Assert.AreEqual(5, result.Setting2);
            Assert.IsFalse(logger.HasWarningsOrErrors);
        }

        [TestMethod]
        public void UpdateConfigSection_should_call_WriteToFile_with_serialized_section_and_fileName()
        {
            var logger = new MockLogger();
            var storage = new MockStorage();
            var sut = new ConfigurationService(storage, logger);

            var section = new TestConfigWithNoDefault { Setting1 = "Value1", Setting2 = 5 };

            sut.UpdateConfigSection(TestConfigWithNoDefault.SectionName, section);

            Assert.AreEqual($"mako-{TestConfigWithNoDefault.SectionName}.cfg".ToLower(), storage.FileName);
            Assert.AreEqual("{\"Setting2\":5,\"Setting1\":\"Value1\",\"SectionName\":\"TestSection\"}", storage.Text);
            Assert.IsFalse(logger.HasWarningsOrErrors);
        }

        [TestMethod]
        public void UpdateConfigSection_should_invoke_ConfigurationUpdated_event()
        {
            var logger = new MockLogger();
            var storage = new MockStorage();
            var sut = new ConfigurationService(storage, logger);

            string sectionUpdated = String.Empty;
            sut.ConfigurationUpdated += (sender, args) =>
            {
                sectionUpdated = (string)(args as ObjectEventArgs).Data;
            };

            var section = new TestConfigWithNoDefault { Setting1 = "Value1", Setting2 = 5 };

            sut.UpdateConfigSection(TestConfigWithNoDefault.SectionName, section);

            Assert.AreEqual(TestConfigWithNoDefault.SectionName, sectionUpdated);
            Assert.IsFalse(logger.HasWarningsOrErrors);
        }

        [TestMethod]
        public void UpdateConfigSection_given_storage_exception_should_not_invoke_ConfigurationUpdated_event()
        {
            var logger = new MockLogger();
            var storage = new MockStorage { ThrowException = true };
            var sut = new ConfigurationService(storage, logger);

            bool eventRaised = false;
            sut.ConfigurationUpdated += (sender, args) =>
            {
                eventRaised = true;
            };

            var section = new TestConfigWithNoDefault { Setting1 = "Value1", Setting2 = 5 };

            sut.UpdateConfigSection(TestConfigWithNoDefault.SectionName, section);

            Assert.IsFalse(eventRaised);
        }

        [TestMethod]
        public void WriteDefault_given_no_file_should_call_WriteToFile_with_serialized_section_and_fileName()
        {
            var logger = new MockLogger();
            var storage = new MockStorage();
            var sut = new ConfigurationService(storage, logger);

            var section = new TestConfigWithNoDefault { Setting1 = "Value1", Setting2 = 5 };

            sut.WriteDefault(TestConfigWithNoDefault.SectionName, section);

            Assert.AreEqual($"mako-{TestConfigWithNoDefault.SectionName}.cfg".ToLower(), storage.FileName);
            Assert.AreEqual("{\"Setting2\":5,\"Setting1\":\"Value1\",\"SectionName\":\"TestSection\"}", storage.Text);
            Assert.IsFalse(logger.HasWarningsOrErrors);
        }

        [TestMethod]
        public void WriteDefault_given_existing_file_should_do_nothing()
        {
            var logger = new MockLogger();
            var storage = new MockStorage { FileName = $"mako-{TestConfigWithNoDefault.SectionName}.cfg".ToLower() };

            var sut = new ConfigurationService(storage, logger);

            var section = new TestConfigWithNoDefault { Setting1 = "Value1", Setting2 = 5 };

            sut.WriteDefault(TestConfigWithNoDefault.SectionName, section);

            Assert.IsNull(storage.Text);
            Assert.IsFalse(logger.HasWarningsOrErrors);
        }

        class TestConfigWithNoDefault
        {
            public string Setting1 { get; set; }
            public int Setting2 { get; set; }

            public static string SectionName => "TestSection";
        }
    }
}
