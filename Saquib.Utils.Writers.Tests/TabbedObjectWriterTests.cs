using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Saquib.Utils.Writers {
    public class TabbedObjectWriterTests {
        record TestObject( string SomeString, double SomeDouble, [property: CurrencyFormat] double SomeCurrency );
        class CurrencyFormatAttribute : FormatAttribute {
            public override string Format( object value ) => $"{value:c}";
        }
        [Fact]
        public async Task WriteBatchAsync__WritesCorrectly() {
            var data = new TestObject[] {
                new("str1", -1.1, -1.1),
                new(null, +1.1, +1.1),
                new("", 0, 0),
            };
            var expected = @"SomeString | SomeDouble | SomeCurrency
======================================
str1       | -1.1       | -$1.10      
           | 1.1        | $1.10       
           | 0          | $0.00       
";

            var written = await WriteData( data );

            Assert.Equal( expected, written );
        }

        private static async Task<string> WriteData( IEnumerable<TestObject> data ) {
            using var writer = new StringWriter();
            writer.NewLine = "\r\n";

            var tdw = new TabbedObjectWriter<TestObject>(
                writer,
                nameof( TestObject.SomeString ),
                nameof( TestObject.SomeDouble ),
                nameof( TestObject.SomeCurrency )
            );
            await tdw.WriteBatchAsync( data );
            return writer.ToString();
        }
    }
}
