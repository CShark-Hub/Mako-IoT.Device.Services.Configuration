using System;
using MakoIoT.Device.Services.Interface;

namespace MakoIoT.Device.Services.Configuration.Test.Mocks
{
    public class MockStorage : IStorageService
    {
        public string FileName { get; set; }
        public string Text { get; set; }

        public bool ThrowException { get; set; }

        public void WriteToFile(string fileName, string text)
        {
            if (ThrowException)
                throw new Exception("Test exception");

            FileName = fileName;
            Text = text;
        }

        public void WriteToFile(string directory, string fileName, string text)
        {
            throw new NotImplementedException();
        }

        public bool FileExists(string fileName)
        {
            return FileName == fileName;
        }

        public bool FileExists(string directory, string fileName)
        {
            throw new NotImplementedException();
        }

        public string ReadFile(string fileName)
        {
            if (FileName == fileName)
                return Text;
            return String.Empty;
        }

        public string ReadFile(string directory, string fileName)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string fileName)
        {
            throw new NotImplementedException();
        }

        public void DeleteFile(string directory, string fileName)
        {
            throw new NotImplementedException();
        }

        public void EnsureDirectory(string directoryName)
        {
            throw new NotImplementedException();
        }

        public string[] GetFiles()
        {
            throw new NotImplementedException();
        }

        public string[] GetFileNames()
        {
            throw new NotImplementedException();
        }

        public string[] GetFiles(string directoryName)
        {
            throw new NotImplementedException();
        }
    }
}
