using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Saquib.Utils.Writers {
    public sealed class JsonLinesWriter {

        private readonly TextWriter _writer;

        public JsonLinesWriter( TextWriter writer ) {
            _writer = writer;
        }

        public async Task WriteAsync<T>( T item ) {
            await _writer.WriteLineAsync( JsonConvert.SerializeObject( item, Formatting.None ) );
        }

        public async Task WriteBatchAsync<T>( IEnumerable<T> items ) {
            foreach ( var obj in items ) {
                await _writer.WriteLineAsync( JsonConvert.SerializeObject( obj, Formatting.None ) );
            }
        }
    }
}