# FilePipeReaderSample

``
using var stream = File.Open("data01.dat", FileMode.Open);
var row = 0;
await FilePipeReader.ReadAllAsync(
    stream,
    new byte[] { (byte)'\r', (byte)'\n' },
    (in ReadOnlySpan<byte> buffer) =>
    {
        Console.WriteLine($"{++row:00000}");
        Console.WriteLine(enc.GetString(buffer));
    });
``
