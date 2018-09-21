using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Saquib.Utils.Writers {
    public sealed class TabbedDataWriter {

        private static readonly string s_columnSeparator = " | ";
        private static readonly char s_dash = '=';

        private readonly TextWriter _writer;

        public IImmutableList<string> Header { get; }

        public TabbedDataWriter( TextWriter writer, IImmutableList<string> header ) {
            _writer = writer;
            Header = header;
        }

        public async Task WriteAsync( string[] items ) {
            var paddedHeader = Header.Select( ( h, i ) => h.PadRight( items[i].Length ) ).ToArray();
            var paddedData = items.Select( ( d, i ) => d.PadRight( paddedHeader[i].Length ) ).ToArray();

            await WriteHeaderLineAsync( paddedHeader );
            await WriteItemsLineAsync( paddedData );
        }

        public async Task WriteBatchAsync( IEnumerable<string[]> data ) {
            var paddedHeader = Header.ToArray();
            foreach ( var items in data ) {
                paddedHeader = paddedHeader.Select( ( header, i ) => header.PadRight( items[i].Length ) ).ToArray();
            }

            data = data.Select( items => items.Select( ( item, i ) => item.PadRight( paddedHeader[i].Length ) ).ToArray() );

            await WriteHeaderLineAsync( paddedHeader );
            foreach ( var items in data ) {
                await WriteItemsLineAsync( items );
            }
        }

        private async Task WriteHeaderLineAsync( IEnumerable<string> items ) {
            var header = string.Join( s_columnSeparator, items );
            await _writer.WriteLineAsync( header );

            var dashedLine = MakeHeaderUnderline( header.Length );
            await _writer.WriteLineAsync( dashedLine );
        }

        private async Task WriteItemsLineAsync( IEnumerable<string> items ) {
            var line = string.Join( s_columnSeparator, items );
            await _writer.WriteLineAsync( line );
        }

        private static string MakeHeaderUnderline( int length ) {
            return new string( Enumerable.Repeat( s_dash, length ).ToArray() );
        }
    }
}
