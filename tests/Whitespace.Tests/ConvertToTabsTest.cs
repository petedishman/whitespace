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

            var result = RunTest(options, contents);
            Assert.Equal(expectedContents, result);
        }




        [Fact]
        public void MixedTabsAndSpacesTurnedToTabs()
        {
            var options = new ConversionOptions()
            {
                LineEndingStyle = LineEnding.Leave,
                TabWidth = 4,
                Indentation = IndentationStyle.Tabs
            };
            var contents = " \t Testing\n\t \tTesting\n";
            var expectedContents = "\t\tTesting\n\t\tTesting\n";
            var result = RunTest(options, contents);
            Assert.Equal(expectedContents, result);
        }

        ConversionOptions ConvertToTabsOptions(int tabWidth)
        {
            return new ConversionOptions()
            {
                Indentation = IndentationStyle.Tabs,
                TabWidth = tabWidth
            };
        }

        [Fact]
        public void CorrectTabsAreUntouched()
        {
            var options = ConvertToTabsOptions(4);
            var sourceText =
"\tIndented text\n" +
"\tCorrectly using tabs\n" +
"\t\tfor indentation\n" +
"\t\tthat shouldn't be changed\n" +
"\tat all\n";
            var expectedText = sourceText;

            RunTestAndCheckResult(options, sourceText, expectedText);
        }

        [Fact]
        public void SpacesThatMatchTabWidthChangeToTabs()
        {
            var options = ConvertToTabsOptions(4);
            var sourceText =
"    text indented with\n"+
"    tab width amount of spaces\n" +
"    are directly replaced\n" +
"        with the right number of tabs\n" +
"        and that's it\n" +
"non-indented lines are obviously left alone";
            var expectedText =
"\ttext indented with\n" +
"\ttab width amount of spaces\n" +
"\tare directly replaced\n" +
"\t\twith the right number of tabs\n" +
"\t\tand that's it\n" +
"non-indented lines are obviously left alone";

            RunTestAndCheckResult(options, sourceText, expectedText);
        }

        [Fact]
        public void LessThanTabWidthSpacesPrecedingTabAreAbsorbedInToTab()
        {
            var options = ConvertToTabsOptions(4);
            var sourceText =
"  \tlines that have\n" +
"   \tless than 4 spaces followed by a tab\n" +
" \tresult in just one tab\n" +
"\tand the same happens even not at the beginning of a line\n" +
" \t  \t   \tthis should be just 3 tabs\n";
            var expectedText =
"\tlines that have\n" +
"\tless than 4 spaces followed by a tab\n" +
"\tresult in just one tab\n" +
"\tand the same happens even not at the beginning of a line\n" +
"\t\t\tthis should be just 3 tabs\n";

            RunTestAndCheckResult(options, sourceText, expectedText);
        }

        [Fact]
        public void TabWidthOfSpacesTurnsToTab()
        {
            var options = ConvertToTabsOptions(4);
            var sourceText =
"    \t    lines indented with spaces and tabs\n"+
"\t    where there are tabwidth spaces turn\n" +
"    \tto tabs\n";
            var expectedText =
"\t\t\tlines indented with spaces and tabs\n" +
"\t\twhere there are tabwidth spaces turn\n" +
"\t\tto tabs\n";

            RunTestAndCheckResult(options, sourceText, expectedText);
        }

        /// <summary>
        /// This ones a bit more controversial.
        /// Current decision is that
        /// \t\t..
        /// with a tab width of 4
        /// should turn to just 2 tabs
        /// and not 3 tabs.
        /// Another option would be
        /// leaving it as \t\t..
        /// i.e. allowing tabs for indentation and spaces for alignment
        /// </summary>
        [Fact]
        public void SpacesLessThanTabWidthFollowingTabsAreDropped()
        {
            var options = ConvertToTabsOptions(4);
            var sourceText =
"\t line 1\n"+
"\t\t  line 2\n" +
"\t   line 3\n" +
"\t\tline 4\n" +
" line 5\n";
            var expectedText =
"\t\tline 1\n" +
"\t\t\tline 2\n" +
"\t\tline 3\n" +
"\t\tline 4\n" +
"\tline 5\n";

            RunTestAndCheckResult(options, sourceText, expectedText);
        }

        /// <summary>
        /// This is similar to the above we're just ensuring
        /// \t\t...... (2 tabs, 6 spaces)
        /// turns to
        /// \t\t\t
        /// </summary>
        [Fact]
        public void SpacesMoreThanTabWidthFollowingTabsTurnToSingleTab()
        {
            var options = ConvertToTabsOptions(4);
            var sourceText =
"\t     line 1\n" +
"\t\t     line 2\n" +
" line 5\n";
            var expectedText =
"\t\t\tline 1\n" +
"\t\t\t\tline 2\n" +
"\tline 5\n";

            RunTestAndCheckResult(options, sourceText, expectedText);
        }
    }
}
