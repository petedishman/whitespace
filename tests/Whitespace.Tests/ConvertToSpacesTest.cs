using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace Whitespace.Tests
{
    public class ConvertToSpacesTest : TestBase
    {
        [Fact]
        public void MixedSpacesAndTabsConversion()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.Leave,
                StripTrailingSpaces = true,
                Indentation = IndentationStyle.Spaces,
                TabWidth = 2
            };
            var contents = " \tTesting";
            var expectedContents = "  Testing";
            var result = RunTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void MixedTabsAndSpacesTurnedToSpaces()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.Leave,
                TabWidth = 4,
                Indentation = IndentationStyle.Spaces
            };
            var contents = " \t Testing\n\t \tTesting\n";
            var expectedContents = "     Testing\n        Testing\n";
            var result = RunTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void TabsTurnedToSpaces()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.Leave,
                TabWidth = 4,
                Indentation = IndentationStyle.Spaces
            };
            var contents = "\tTesting\n\t\tTesting\n";
            var expectedContents = "    Testing\n        Testing\n";
            var result = RunTest(options, contents);
            Assert.Equal(expectedContents, result);
        }
    }
}
