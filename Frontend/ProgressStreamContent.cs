using System.Net;
using System.Net.Http;

public class ProgressStreamContent : StreamContent
{
    private readonly Stream _stream;
    private readonly Action<UploadProgress> _onProgress;

    public ProgressStreamContent(HttpContent content, Action<UploadProgress> onProgress)
        : base(GetStream(content))
    {
        _stream = GetStream(content);
        _onProgress = onProgress;

        foreach (var header in content.Headers)
        {
            Headers.Add(header.Key, header.Value);
        }
    }

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        var buffer = new byte[8192];
        var totalBytesRead = 0L;
        var totalBytes = _stream.Length;
        int bytesRead;

        while ((bytesRead = await _stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            await stream.WriteAsync(buffer, 0, bytesRead);
            await Task.Delay(16);
            totalBytesRead += bytesRead;

            _onProgress(new UploadProgress 
            { 
                BytesTransferred = totalBytesRead,
                TotalBytes = totalBytes
            });
        }
    }

    private static Stream GetStream(HttpContent content)
    {
        var task = content.ReadAsStreamAsync();
        task.Wait();
        return task.Result;
    }

    public class UploadProgress
    {
        public long BytesTransferred { get; set; }
        public long TotalBytes { get; set; }
    }
}