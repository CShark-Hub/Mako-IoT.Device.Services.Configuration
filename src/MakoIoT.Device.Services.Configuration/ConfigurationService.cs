using System;
using System.Collections;
using System.Reflection;
using MakoIoT.Device.Services.Interface;
using nanoFramework.Json;

namespace MakoIoT.Device.Services.Configuration
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly IStorageService _storage;
        private readonly ILog _logger;
        private string _configString;
        private readonly object _writeLock = new object();

        public event EventHandler ConfigurationUpdated;

        public ConfigurationService(IStorageService storage, ILog logger)
        {
            _storage = storage;
            _logger = logger;
        }

        public object GetConfigSection(string sectionName, Type objectType)
        {
            string sectionStr = LoadConfigSection(sectionName);

            if (sectionStr != null)
            {
                try
                {
                    return JsonConvert.DeserializeObject(sectionStr, objectType);
                }
                catch (Exception e)
                {
                    _logger.Error($"Error in config section {sectionName}", e);
                }
            }

            var defaultProp = objectType.GetMethod("get_Default", BindingFlags.Public | BindingFlags.Static);
            if (defaultProp != null)
            {
                _logger.Information($"Using default config for section {sectionName}");
                return defaultProp.Invoke(null, null);
            }

            throw new ConfigurationException("Can't load configuration nor default");
        }

        public void UpdateConfigSection(string sectionName, object section)
        {
            string sectionStr = JsonConvert.SerializeObject(section);
            if (SaveConfigSection(sectionStr, sectionName))
            {
                ConfigurationUpdated?.Invoke(this, new ObjectEventArgs(sectionName));
            }
        }

        public bool UpdateConfigSectionString(string sectionName, string sectionString)
        {
            bool result = SaveConfigSection(sectionString, sectionName);
            if (result)
            {
                ConfigurationUpdated?.Invoke(this, new ObjectEventArgs(sectionName));
            }

            return result;
        }

        public bool UpdateConfigSectionString(string sectionName, string sectionString, Type objectType)
        {
            try
            {
                JsonConvert.DeserializeObject(sectionString, objectType);
            }
            catch (Exception e)
            {
                _logger.Error($"Error in config section {sectionName} - config not updated", e);
                return false;
            }
            if (SaveConfigSection(sectionString, sectionName))
            {
                ConfigurationUpdated?.Invoke(this, new ObjectEventArgs(sectionName));
            }
            return true;
        }

        public void WriteDefault(string sectionName, object section, bool overwrite = false)
        {
            if (overwrite || !_storage.FileExists(GetConfigFileName(sectionName)))
                UpdateConfigSection(sectionName, section);
        }

        public string[] GetSections()
        {
            var sections = new ArrayList();
            var files = _storage.GetFileNames();
            foreach (var file in files)
            {
                if (IsConfigFile(file))
                    sections.Add(file.Substring(5, file.Length - 9));
            }

            return (string[])sections.ToArray(typeof(string));
        }

        public string LoadConfigSection(string sectionName)
        {
            string fileName = GetConfigFileName(sectionName);
            if (_storage.FileExists(fileName))
            {
                try
                {
                    _configString = _storage.ReadFile(fileName);
                    return _configString;
                }
                catch (Exception e)
                {
                    _logger.Error("Error loading config from file", e);
                }
            }
            return null;
        }

        public bool ClearAll()
        {
            var result = true;
            var files = _storage.GetFileNames();
            foreach (var file in files)
            {
                if (IsConfigFile(file))
                {
                    _logger.Trace($"Deleting file {file}...");
                    try
                    {
                        _storage.DeleteFile(file);
                    }
                    catch (Exception e)
                    {
                        _logger.Error($"Error deleting config file {file}", e);
                        result = false;
                    }
                }
            }

            return result;
        }

        private bool SaveConfigSection(string config, string sectionName)
        {
            string fileName = GetConfigFileName(sectionName);
            try
            {
                lock (_writeLock)
                {
                    _storage.WriteToFile(fileName, config);
                    _logger.Trace($"Config section {sectionName} updated.");
                }

                return true;
            }
            catch (Exception e)
            {
                _logger.Error("Error saving config to file", e);
            }

            return false;
        }

        private static string GetConfigFileName(string sectionName) => $"mako-{sectionName}.cfg".ToLower();

        private static bool IsConfigFile(string name) => name.StartsWith("mako-") && name.EndsWith(".cfg");

    }

    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message) : base(message)
        {

        }
    }
}
