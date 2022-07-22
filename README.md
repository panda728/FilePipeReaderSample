# FilePipeReaderSample

```
var row = 0;
await FilePipeReader.ReadAllAsync(
    "data01.dat",
    new byte[] { (byte)'\r', (byte)'\n' },
    (in ReadOnlySpan<byte> buffer) =>
    {
        Console.WriteLine($"{++row:00000}");
        Console.WriteLine(enc.GetString(buffer));
    });
```
