using System;

namespace Whitespace
{
    /// <summary>
    /// Raised when there's an error with the command line arguments
    /// </summary>
    public class ConfigurationException : Exception
    {
        public ConfigurationException(string message)
            : base(message) { }
    }
}
