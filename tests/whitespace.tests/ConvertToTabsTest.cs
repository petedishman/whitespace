using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace whitespace.tests
{
    public class ConvertToTabsTest : TestBase
    {
        [Fact]
        public void BlankLinesLeftAlone()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.CRLF,
                StripTrailingSpaces = true,
                Type = ConversionType.SpacesToTabs
            };
            var contents = @"line 1
line2

line3

line4
";
            var expectedContents = @"line 1
line2

line3

line4
";

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
    }
}
