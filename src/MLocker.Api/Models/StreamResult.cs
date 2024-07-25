using System;
using System.IO;

namespace MLocker.Api.Models;

public class StreamResult(Stream stream) : IDisposable
{
    public Stream Stream => stream;

    public void Dispose()
    {
        stream.Close();
        stream.Dispose();
    }
}