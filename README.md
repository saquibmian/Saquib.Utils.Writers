# Saquib.Utils.Writers
Various object writers for .NET.

## TabbedDataWriter

Here is a sample usage:

```csharp
var header = ImmutableHashSet.Create<string>( "id", "some_other_id", "name" );
var data = new[] {
    new[] { "1", "2", "first" },
    new[] { "2", "2", "second" },
};

var tdw = new TabbedDataWriter( Console.Out, header );
await tdw.WriteBatchAsync( data );
```

Outputs the following:

```
id | some_other_id | name
===========================
1  | 2             | first
2  | 2             | second
```