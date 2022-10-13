using System;
using System.Collections;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace MakoIoT.Device.Services.Configuration.Test.Mocks
{
    public class MockLogger : ILogger
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

        public void Log(LogLevel logLevel, EventId eventId, string state, Exception exception, MethodInfo format)
        {
            _messages[(int)logLevel].Add(state);
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public ArrayList GetMessages(LogLevel logLevel)
        {
            return _messages[(int)logLevel];
        }

        public ArrayList GetAllMessages()
        {
            var l = new ArrayList();
            foreach (var message in _messages)
            {
                foreach (var m in message)
                {
                    l.Add(m);
                }
            }

            return l;
        }

        public bool HasErrors => GetMessages(LogLevel.Error).Count > 0;

        public bool HasWarningsOrErrors =>
            GetMessages(LogLevel.Error).Count > 0 || GetMessages(LogLevel.Warning).Count > 0;
    }
}
