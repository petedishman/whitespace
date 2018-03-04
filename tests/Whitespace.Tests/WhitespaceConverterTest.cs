using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace Whitespace.Tests
{
    public class TestBase
    {
        protected string RunTest(ConversionOptions options, string fileText)
        {
            var converter = new WhitespaceConverter(options);
            return converter.ConvertFileText(fileText);
        }

        protected void RunTestAndCheckResult(ConversionOptions options, string sourceText, string expectedText)
        {
            var result = RunTest(options, sourceText);
            Assert.Equal(expectedText, result);
        }
    }
}
