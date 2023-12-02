using MakoIoT.Device.Services.Interface;
using System;
using System.Collections;
using System.Reflection;

namespace MakoIoT.Device.Services.Configuration.Test.Mocks
{
    public class MockLogger : ILog
    {
        private readonly ArrayList[] _messages;

        public MockLogger()
        {
            _messages = new ArrayList[6];
            for (int i = 0; i < 6; i++)
            {
                _messages[i] = new ArrayList();
            }
        }

        private void Log(LogEventLevel logLevel, string state)
        {
            _messages[(int)logLevel].Add(state);
        }

        public ArrayList GetMessages(LogEventLevel logLevel)
        {
            return _messages[(int)logLevel];
        }


        public void Trace(Exception exception, string message, MethodInfo format)
        {
            Log(LogEventLevel.Trace, message);
        }

        public void Trace(Exception exception, string message)
        {
            Log(LogEventLevel.Trace, message);
        }

        public void Trace(string message)
        {
            Log(LogEventLevel.Trace, message);
        }

        public void Trace(Exception exception)
        {
            Log(LogEventLevel.Trace, exception.ToString());
        }

        public void Information(Exception exception, string message, MethodInfo format)
        {
            Log(LogEventLevel.Information, message);
        }

        public void Information(Exception exception, string message)
        {
            Log(LogEventLevel.Information, message);
        }

        public void Information(string message)
        {
            Log(LogEventLevel.Information, message);
        }

        public void Information(Exception exception)
        {
            Log(LogEventLevel.Information, exception.ToString());
        }

        public void Warning(Exception exception, string message, MethodInfo format)
        {
            Log(LogEventLevel.Warning, message);
        }

        public void Warning(Exception exception, string message)
        {
            Log(LogEventLevel.Warning, message);
        }

        public void Warning(string message)
        {
            Log(LogEventLevel.Warning, message);
        }

        public void Warning(Exception exception)
        {
            Log(LogEventLevel.Warning, exception.ToString());
        }

        public void Error(Exception exception, string message, MethodInfo format)
        {
            Log(LogEventLevel.Error, message);
        }

        public void Error(string message, Exception exception)
        {
            Log(LogEventLevel.Error, message);
        }

        public void Error(string message)
        {
            Log(LogEventLevel.Error, message);
        }

        public void Error(Exception exception)
        {
            Log(LogEventLevel.Error, exception.ToString());
        }

        public void Critical(Exception exception, string message, MethodInfo format)
        {
            Log(LogEventLevel.Critical, message);
        }

        public void Critical(Exception exception, string message)
        {
            Log(LogEventLevel.Critical, message);
        }

        public void Critical(string message)
        {
            Log(LogEventLevel.Critical, message);
        }

        public void Critical(Exception exception)
        {
            Log(LogEventLevel.Critical, exception.Message);
        }

        public bool HasWarningsOrErrors =>
            GetMessages(LogEventLevel.Error).Count > 0 || GetMessages(LogEventLevel.Warning).Count > 0;
    }
}
