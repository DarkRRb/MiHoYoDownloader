using System;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace MiHoYoDownloader.Utilities;

public class AsyncConsole : IDisposable {
    private readonly ChannelReader<Output> _reader;
    private readonly ChannelWriter<Output> _writer;
    private readonly Task _loop;

    public AsyncConsole() {
        Channel<Output> channel = Channel.CreateUnbounded<Output>(new UnboundedChannelOptions {
            AllowSynchronousContinuations = false,
            SingleReader = true,
            SingleWriter = false,
        });

        _reader = channel.Reader;
        _writer = channel.Writer;

        _loop = OutputLoopAsync();
    }

    public async Task OutputLoopAsync() {
        try {
            await foreach (Output output in _reader.ReadAllAsync()) {
                Console.Write(output.Format, output.Args);
            }
        } catch (Exception e) {
            Console.WriteLine(e);
            Environment.Exit(-1);
        }
    }

    public ValueTask WriteAsync(string format, params object?[]? args) {
        return _writer.WriteAsync(new Output(format, args));
    }

    public ValueTask WriteLineAsync() {
        return _writer.WriteAsync(new Output("\n", null));
    }

    public void Dispose() {
        _writer.Complete();
        _loop.Wait();

        GC.SuppressFinalize(this);
    }

    private class Output(string format, object?[]? args) {
        public string Format { get; } = format;

        public object?[]? Args { get; } = args;
    }
}