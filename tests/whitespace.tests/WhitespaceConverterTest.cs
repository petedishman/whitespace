using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace whitespace.tests
{
    public class TestBase
    {
        protected string RunConversionTest(ConversionOptions options, string fileText)
        {
            var converter = new WhitespaceConverter(options);
            return converter.ConvertFileText(fileText);
        }
    }
}
