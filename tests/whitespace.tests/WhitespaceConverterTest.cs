using System;
using Xunit;
using System.Collections.Generic;
using System.Text;

namespace whitespace.tests
{
    public class WhitespaceConverterTest
    {
        [Fact]
        public void Test1()
        {
            var converter = new WhitespaceConverter(new ConversionOptions());
            byte[] line = new byte[100];

            var result = converter.ConvertLine(line, false);
        }
    }
}
