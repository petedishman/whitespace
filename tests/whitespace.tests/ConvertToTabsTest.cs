using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace Whitespace.Tests
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
                Indentation = IndentationStyle.Tabs
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
                Indentation = IndentationStyle.Tabs
            };
            var contents = " \t Testing\n\t \tTesting\n";
            var expectedContents = "\t\tTesting\n\t\t\tTesting\n";
            var result = RunConversionTest(options, contents);
            Assert.Equal(expectedContents, result);
        }
    }
}
