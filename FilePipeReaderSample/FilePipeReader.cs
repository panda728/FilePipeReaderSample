using System.Buffers;
using System.IO.Pipelines;

namespace FilePipeReaderSample
{
    public class FilePipeReader
    {
        public delegate void ReadTo(in ReadOnlySpan<byte> buffer);

        public static async Task ReadAllAsync(Stream input, Memory<byte> delimiter, ReadTo readTo)
        {
            var pipe = new Pipe();
            var writing = FillPipeAsync(input, pipe.Writer);
            var reading = ReadPipeAsync(pipe.Reader, delimiter, readTo);
            await Task.WhenAll(writing, reading);
        }

        static async Task FillPipeAsync(Stream input, PipeWriter writer)
        {
            while (true)
            {
                try
                {
                    var memory = writer.GetMemory();
                    int byteRead = await input.ReadAsync(memory);
                    if (byteRead == 0)
                        break;
                    writer.Advance(byteRead);
                }
                catch
                {
                    break;
                }
                var result = await writer.FlushAsync();
                if (result.IsCompleted)
                    break;
            }
            writer.Complete();
        }

        static async Task ReadPipeAsync(PipeReader reader, Memory<byte> delimiter, ReadTo readTo)
        {
            while (true)
            {
                var result = await reader.ReadAsync();
                var buffer = result.Buffer;
                ReadBuffer(ref buffer, delimiter, readTo);
                reader.AdvanceTo(buffer.Start, buffer.End);
                if (result.IsCompleted)
                    break;
            }
            reader.Complete();
        }

        static void ReadBuffer(ref ReadOnlySequence<byte> buffer, Memory<byte> delimiter, ReadTo readTo)
        {
            var sequenceReader = new SequenceReader<byte>(buffer);
            while (!sequenceReader.End)
            {
                while (sequenceReader.TryReadTo(out ReadOnlySpan<byte> line, delimiter.Span))
                    readTo(line);

                buffer = buffer.Slice(sequenceReader.Position);
                sequenceReader.Advance(buffer.Length);
            }
        }
    }

}
