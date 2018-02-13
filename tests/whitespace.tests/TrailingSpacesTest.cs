using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace whitespace.tests
{
    public class TrailingSpacesTest : TestBase
    {
        [Fact]
        public void TrailingSpacesRemoved()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.None,
                StripTrailingSpaces = true,
                Indentation = IndentationStyle.Spaces
            };
            var contents = "Testing\t ";
            var expectedContents = "Testing";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void TrailingSpacesRemovedFromMultipleLines()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.None,
                StripTrailingSpaces = true,
                Indentation = IndentationStyle.Spaces
            };
            var contents = "Testing\t \nTesting 1,2,3   \n\ntest 1";
            var expectedContents = "Testing\nTesting 1,2,3\n\ntest 1";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void SolitarySpaceRemovedWhenConvertingToTabs()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.LF,
                StripTrailingSpaces = true,
                Indentation = IndentationStyle.Tabs
            };
            var contents = "testing\n \n\ntesting\n";
            var expected = "testing\n\n\ntesting\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expected, result);
        }

        [Fact]
        public void SolitarySpaceRemovedWhenConvertingToTabsWithCrlf()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.CRLF,
                StripTrailingSpaces = true,
                Indentation = IndentationStyle.Tabs
            };
            var contents = "testing\r\n \r\n\r\ntesting\r\n";
            var expected = "testing\r\n\r\n\r\ntesting\r\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expected, result);
        }

    }
}
