using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Saquib.Utils.Writers {
    public abstract class FormatAttribute : Attribute {
        public abstract string? Format( object? value );
    }
    public sealed class TabbedObjectWriter<T> {
        private readonly TabbedDataWriter _inner;
        private readonly ObjectProperty[] _properties;

        public string NullValuePlaceholder { get; set; } = "";

        public TabbedObjectWriter( TextWriter writer, params string[] properties ) {
            _properties = GetObjectProperties( properties ).ToArray();
            _inner = new TabbedDataWriter( writer, _properties.Select( p => p.Name ).ToImmutableArray() );
        }

        public Task WriteAsync( T item ) => _inner.WriteAsync(
            _properties
                .Select( prop => prop.Getter( item ) ?? NullValuePlaceholder )
                .ToArray()
        );
        public Task WriteBatchAsync( IEnumerable<T> items ) => _inner.WriteBatchAsync(
            items.Select(
                item => _properties
                    .Select( prop => prop.Getter( item ) ?? NullValuePlaceholder )
                    .ToArray()
            )
        );

        private static IEnumerable<ObjectProperty> GetObjectProperties( params string[] names ) {
            var type = typeof( T );
            var formatterType = typeof( FormatAttribute );
            foreach ( var name in names ) {
                var prop = type.GetProperty( name ) ?? throw new ArgumentException( "No property found with name $p", nameof( names ) );
                var formatter = prop.GetCustomAttributes()
                    .Where( a => formatterType.IsAssignableFrom( a.GetType() ) )
                    .Cast<FormatAttribute>()
                    .SingleOrDefault();
                Func<T, string?> getter = formatter is null
                    ? ( T item ) => prop.GetValue( item )?.ToString()
                    : ( T item ) => formatter.Format( prop.GetValue( item ) );
                yield return new( prop.Name, getter );
            }
        }
        private record struct ObjectProperty( string Name, Func<T, string?> Getter );
    }
}
