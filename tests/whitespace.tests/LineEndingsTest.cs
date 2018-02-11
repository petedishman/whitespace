using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace whitespace.tests
{
    public class LineEndingsTest : TestBase
    {
        [Fact]
        public void LineEndingsNormalizedToCrlf()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.CRLF,
                Type = ConversionType.TabsToSpaces
            };
            var contents = "Testing\nTesting\n";
            var expectedContents = "Testing\r\nTesting\r\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void LineEndingsNormalizedToLf()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.LF,
                Type = ConversionType.TabsToSpaces
            };
            var contents = "Testing\r\nTesting\r\n";
            var expectedContents = "Testing\nTesting\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void LineEndingsLeftAlone()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.None,
                Type = ConversionType.TabsToSpaces
            };
            var contents = "Testing\nTesting\r\n";
            var expectedContents = "Testing\nTesting\r\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void LineEndingsNormalizedToCrlfWithStripTrailingSpacesOn()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.CRLF,
                StripTrailingSpaces = true,
                Type = ConversionType.TabsToSpaces
            };
            var contents = "Testing\nTesting\n";
            var expectedContents = "Testing\r\nTesting\r\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void LineEndingsNormalizedToLfWithStripTrailingSpacesOn()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.LF,
                StripTrailingSpaces = true,
                Type = ConversionType.TabsToSpaces
            };
            var contents = "Testing\r\nTesting\r\n";
            var expectedContents = "Testing\nTesting\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void LineEndingsLeftAloneWithStripTrailingSpacesOn()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.None,
                StripTrailingSpaces = true,
                Type = ConversionType.TabsToSpaces
            };
            var contents = "Testing\nTesting\r\n";
            var expectedContents = "Testing\nTesting\r\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }


    }
}
