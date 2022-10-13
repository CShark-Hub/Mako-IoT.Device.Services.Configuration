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
        // [TestMethod]
        // public void GetConfigSection_given_NoFile_ConfigClassWithDefault_should_return_Default()
        // {
        //     var sut = new ConfigurationService(new MockStorage(), new DebugLogger(nameof(ConfigurationServiceTest)));
        //
        //     var result = sut.GetConfigSection(DataProvidersConfig.SectionName, typeof(DataProvidersConfig)) as DataProvidersConfig;
        //
        //     Assert.NotNull(result);
        //     Assert.Equal(DataProvidersConfig.Default.Providers[0].DataProviderId, result.Providers[0].DataProviderId);
        // }

        // [TestMethod]
        // public void GetConfigSection_given_NoFile_ConfigClassWithDefaultInPartialClass_should_return_Default()
        // {
        //     var sut = new ConfigurationService(new MockStorage(), new DebugLogger(nameof(ConfigurationServiceTest)));
        //
        //     var result = sut.GetConfigSection(WiFiConfig.SectionName, typeof(WiFiConfig)) as WiFiConfig;
        //
        //     Assert.NotNull(result);
        //     Assert.Equal(WiFiConfig.Default.Ssid, result.Ssid);
        // }

        [TestMethod]
        public void GetConfigSection_given_NoFile_ConfigClassWithNoDefault_should_throw_ConfigurationException()
        {
            var sut = new ConfigurationService(new MockStorage(), new DebugLogger(nameof(ConfigurationServiceTest)));

            Assert.Throws(typeof(ConfigurationException),
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

            Assert.NotNull(result);
            Assert.Equal("Value1", result.Setting1);
            Assert.Equal(5, result.Setting2);
            Assert.False(logger.HasWarningsOrErrors);
        }

        [TestMethod]
        public void UpdateConfigSection_should_call_WriteToFile_with_serialized_section_and_fileName()
        {
            var logger = new MockLogger();
            var storage = new MockStorage();
            var sut = new ConfigurationService(storage, logger);

            var section = new TestConfigWithNoDefault { Setting1 = "Value1", Setting2 = 5 };

            sut.UpdateConfigSection(TestConfigWithNoDefault.SectionName, section);

            Assert.Equal($"mako-{TestConfigWithNoDefault.SectionName}.cfg".ToLower(), storage.FileName);
            Assert.Equal("{\"Setting2\":5,\"Setting1\":\"Value1\",\"SectionName\":\"TestSection\"}", storage.Text);
            Assert.False(logger.HasWarningsOrErrors);
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

            Assert.Equal(TestConfigWithNoDefault.SectionName, sectionUpdated);

            Assert.False(logger.HasWarningsOrErrors);
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

            Assert.False(eventRaised);
        }

        [TestMethod]
        public void WriteDefault_given_no_file_should_call_WriteToFile_with_serialized_section_and_fileName()
        {
            var logger = new MockLogger();
            var storage = new MockStorage();
            var sut = new ConfigurationService(storage, logger);

            var section = new TestConfigWithNoDefault { Setting1 = "Value1", Setting2 = 5 };

            sut.WriteDefault(TestConfigWithNoDefault.SectionName, section);

            Assert.Equal($"mako-{TestConfigWithNoDefault.SectionName}.cfg".ToLower(), storage.FileName);
            Assert.Equal("{\"Setting2\":5,\"Setting1\":\"Value1\",\"SectionName\":\"TestSection\"}", storage.Text);
            Assert.False(logger.HasWarningsOrErrors);
        }

        [TestMethod]
        public void WriteDefault_given_existing_file_should_do_nothing()
        {
            var logger = new MockLogger();
            var storage = new MockStorage { FileName = $"mako-{TestConfigWithNoDefault.SectionName}.cfg".ToLower() };

            var sut = new ConfigurationService(storage, logger);

            var section = new TestConfigWithNoDefault { Setting1 = "Value1", Setting2 = 5 };

            sut.WriteDefault(TestConfigWithNoDefault.SectionName, section);

            Assert.Null(storage.Text);
            Assert.False(logger.HasWarningsOrErrors);
        }

        // [TestMethod]
        // public void WriteDefault_given_MqttConfig_should_call_WriteToFile_with_serialized_section_and_fileName()
        // {
        //     var logger = new MockLogger();
        //     var storage = new MockStorage();
        //     var sut = new ConfigurationService(storage, logger);
        //
        //     var section = new MqttConfig
        //     {
        //         BrokerAddress = "test.mosquitto.org",
        //         Port = 8883,
        //         UseTLS = true,
        //         CACert = @"-----BEGIN CERTIFICATE-----
        //         MIIEAzCCAuugAwIBAgIUBY1hlCGvdj4NhBXkZ/uLUZNILAwwDQYJKoZIhvcNAQEL
        //         BQAwgZAxCzAJBgNVBAYTAkdCMRcwFQYDVQQIDA5Vbml0ZWQgS2luZ2RvbTEOMAwG
        //         A1UEBwwFRGVyYnkxEjAQBgNVBAoMCU1vc3F1aXR0bzELMAkGA1UECwwCQ0ExFjAU
        //         BgNVBAMMDW1vc3F1aXR0by5vcmcxHzAdBgkqhkiG9w0BCQEWEHJvZ2VyQGF0Y2hv
        //         by5vcmcwHhcNMjAwNjA5MTEwNjM5WhcNMzAwNjA3MTEwNjM5WjCBkDELMAkGA1UE
        //         BhMCR0IxFzAVBgNVBAgMDlVuaXRlZCBLaW5nZG9tMQ4wDAYDVQQHDAVEZXJieTES
        //         MBAGA1UECgwJTW9zcXVpdHRvMQswCQYDVQQLDAJDQTEWMBQGA1UEAwwNbW9zcXVp
        //         dHRvLm9yZzEfMB0GCSqGSIb3DQEJARYQcm9nZXJAYXRjaG9vLm9yZzCCASIwDQYJ
        //         KoZIhvcNAQEBBQADggEPADCCAQoCggEBAME0HKmIzfTOwkKLT3THHe+ObdizamPg
        //         UZmD64Tf3zJdNeYGYn4CEXbyP6fy3tWc8S2boW6dzrH8SdFf9uo320GJA9B7U1FW
        //         Te3xda/Lm3JFfaHjkWw7jBwcauQZjpGINHapHRlpiCZsquAthOgxW9SgDgYlGzEA
        //         s06pkEFiMw+qDfLo/sxFKB6vQlFekMeCymjLCbNwPJyqyhFmPWwio/PDMruBTzPH
        //         3cioBnrJWKXc3OjXdLGFJOfj7pP0j/dr2LH72eSvv3PQQFl90CZPFhrCUcRHSSxo
        //         E6yjGOdnz7f6PveLIB574kQORwt8ePn0yidrTC1ictikED3nHYhMUOUCAwEAAaNT
        //         MFEwHQYDVR0OBBYEFPVV6xBUFPiGKDyo5V3+Hbh4N9YSMB8GA1UdIwQYMBaAFPVV
        //         6xBUFPiGKDyo5V3+Hbh4N9YSMA8GA1UdEwEB/wQFMAMBAf8wDQYJKoZIhvcNAQEL
        //         BQADggEBAGa9kS21N70ThM6/Hj9D7mbVxKLBjVWe2TPsGfbl3rEDfZ+OKRZ2j6AC
        //         6r7jb4TZO3dzF2p6dgbrlU71Y/4K0TdzIjRj3cQ3KSm41JvUQ0hZ/c04iGDg/xWf
        //         +pp58nfPAYwuerruPNWmlStWAXf0UTqRtg4hQDWBuUFDJTuWuuBvEXudz74eh/wK
        //         sMwfu1HFvjy5Z0iMDU8PUDepjVolOCue9ashlS4EB5IECdSR2TItnAIiIwimx839
        //         LdUdRudafMu5T5Xma182OC0/u/xRlEm+tvKGGmfFcN0piqVl8OrSPBgIlb+1IKJE
        //         m/XriWr/Cq4h/JfB7NTsezVslgkBaoU=
        //         -----END CERTIFICATE-----",
        //         ClientId = "device1",
        //         TopicPrefix = "mako-iot-test"
        //     };
        //
        //     sut.WriteDefault(MqttConfig.SectionName, section);
        //
        //     Debug.WriteLine(storage.Text);
        //
        //     var obj = (MqttConfig)JsonConvert.DeserializeObject(storage.Text, typeof(MqttConfig));
        //
        //     Assert.Equal("test.mosquitto.org", obj.BrokerAddress);
        //
        //     Assert.Equal($"mako-{MqttConfig.SectionName}.cfg".ToLower(), storage.FileName);
        //     Assert.Contains("\"BrokerAddress\":\"test.mosquitto.org\"", storage.Text);
        //     Assert.False(logger.HasWarningsOrErrors);
        // }

        class TestConfigWithNoDefault
        {
            public string Setting1 { get; set; }
            public int Setting2 { get; set; }

            public static string SectionName => "TestSection";
        }
    }
}
