using FilePipeReaderSample;
using System.Text;

Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
var enc = Encoding.GetEncoding("shift-jis");

var row = 0;

await FilePipeReader.ReadAllAsync(
    "data01.dat",
    new byte[] { (byte)'\r', (byte)'\n' },
    (in ReadOnlySpan<byte> buffer) =>
    {
        Console.WriteLine($"{++row:00}");
        Console.Write(enc.GetString(buffer[9..69]));
        Console.WriteLine(enc.GetString(buffer[169..247]));
    });
