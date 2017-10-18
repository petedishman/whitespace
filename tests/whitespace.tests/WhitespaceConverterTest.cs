using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace whitespace.tests
{
    public class WhitespaceConverterTest
    {
        [Fact]
        public void TrailingSpacesRemoved()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.None,
                StripTrailingSpaces = true,
                Type = ConversionType.TabsToSpaces
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
                Type = ConversionType.TabsToSpaces
            };
            var contents = "Testing\t \nTesting 1,2,3   ";
            var expectedContents = "Testing\nTesting 1,2,3";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

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
        public void TabsTurnedToSpaces()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.None,
                TabWidth = 4,
                Type = ConversionType.TabsToSpaces
            };
            var contents = "\tTesting\n\t\tTesting\n";
            var expectedContents = "    Testing\n        Testing\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        [Fact]
        public void MixedTabsAndSpacesTurnedToSpaces()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.None,
                TabWidth = 4,
                Type = ConversionType.TabsToSpaces
            };
            var contents = " \t Testing\n\t \tTesting\n";
            var expectedContents = "      Testing\n         Testing\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }
        [Fact]
        public void MixedTabsAndSpacesTurnedToTabs()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.None,
                TabWidth = 4,
                Type = ConversionType.SpacesToTabs
            };
            var contents = " \t Testing\n\t \tTesting\n";
            var expectedContents = "\t\tTesting\n\t\t\tTesting\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        private string RunConversionTest(ConversionOptions options, string fileText)
        {
            var converter = new WhitespaceConverter(options);
            return converter.ConvertFileText(fileText);
        }
    }
}
