using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace whitespace.tests
{
    public class ConvertToSpacesTest : TestBase
    {
        [Fact]
        public void MixedSpacesAndTabsConversion()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.None,
                StripTrailingSpaces = true,
                Type = ConversionType.TabsToSpaces,
                TabWidth = 2
            };
            var contents = " \tTesting";
            var expectedContents = "  Testing";
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
            var expectedContents = "     Testing\n        Testing\n";
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
    }
}
