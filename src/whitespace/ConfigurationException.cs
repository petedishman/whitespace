using System;

namespace Whitespace
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message)
            : base(message) { }
    }
}
