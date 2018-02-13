using System;

namespace whitespace
{
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message)
            : base(message) { }
    }
}
