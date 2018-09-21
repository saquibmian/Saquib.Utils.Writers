using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Xunit;

namespace Saquib.Utils.Writers.Tests {
    public class TabbedDataWriterTests {
        [Fact]
        public async Task WriteBatchAsync__WritesCorrectly() {
            var header = ImmutableArray.Create<string>( "id", "some_other_id", "name" );
            var data = new[] {
                    new[] { "1", "2", "first" },
                    new[] { "2", "2", "second" },
                };
            var expected = @"id | some_other_id | name  
===========================
1  | 2             | first 
2  | 2             | second
";

            var written = await WriteData( header, data );

            Assert.Equal( expected, written );
        }

        private async Task<string> WriteData(
            IImmutableList<string> header,
            IEnumerable<string[]> data
        ) {
            using ( var writer = new StringWriter() ) {
                writer.NewLine = "\r\n";

                var tdw = new TabbedDataWriter( writer, header );
                await tdw.WriteBatchAsync( data );
                return writer.ToString();
            }
        }
    }
}
