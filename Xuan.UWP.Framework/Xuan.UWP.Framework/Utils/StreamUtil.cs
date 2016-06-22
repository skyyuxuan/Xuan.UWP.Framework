using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace Xuan.UWP.Framework.Utils
{
    public static class StreamUtil
    {
        public static async Task<IRandomAccessStream> StreamToRandomStream(Stream stream)
        {
            var randomAccessStream = new InMemoryRandomAccessStream();
            var outputStream = randomAccessStream.GetOutputStreamAt(0);
            await RandomAccessStream.CopyAsync(stream.AsInputStream(), outputStream);
            return randomAccessStream;
        }
        public static Stream RandomStreamToStream(IRandomAccessStream randomStream)
        {
            return WindowsRuntimeStreamExtensions.AsStreamForRead(randomStream.GetInputStreamAt(0));
        }

        public static Stream BufferToStream(IBuffer buffer)
        {
            return WindowsRuntimeBufferExtensions.AsStream(buffer);
        }

        public static IBuffer StreamToBuffer(Stream stream)
        {
            MemoryStream memoryStream = new MemoryStream();
            if (stream != null)
            {
                byte[] bytes = StreamToBytes(stream);
                if (bytes != null)
                {
                    var binaryWriter = new BinaryWriter(memoryStream);
                    binaryWriter.Write(bytes);
                }
            }
            return WindowsRuntimeBufferExtensions.GetWindowsRuntimeBuffer(memoryStream, 0, (int)memoryStream.Length);
        }

        public static byte[] BufferToBytes(IBuffer buffer)
        {
            return WindowsRuntimeBufferExtensions.ToArray(buffer);
        }

        public static IBuffer BytesToBuffer(byte[] bytes)
        {
            return WindowsRuntimeBufferExtensions.AsBuffer(bytes, 0, bytes.Length);
        }
        public static async Task<IRandomAccessStream> BufferToRandomStream(IBuffer buffer)
        {
            InMemoryRandomAccessStream randomAccessStream = new InMemoryRandomAccessStream();
            DataWriter datawriter = new DataWriter(randomAccessStream.GetOutputStreamAt(0));
            datawriter.WriteBuffer(buffer, 0, buffer.Length);
            await datawriter.StoreAsync();
            return randomAccessStream;
        }
        public static IBuffer RandomStreamToBuffer(IRandomAccessStream randomStream)
        {
            Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(randomStream.GetInputStreamAt(0));
            MemoryStream memoryStream = new MemoryStream();
            if (stream != null)
            {
                byte[] bytes = StreamToBytes(stream);
                if (bytes != null)
                {
                    var binaryWriter = new BinaryWriter(memoryStream);
                    binaryWriter.Write(bytes);
                }
            }
            return WindowsRuntimeBufferExtensions.GetWindowsRuntimeBuffer(memoryStream, 0, (int)memoryStream.Length);
        }

        public static byte[] StreamToBytes(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
        public static Stream BytesToStream(byte[] bytes)
        {
            Stream stream = new MemoryStream(bytes);
            return stream;
        }
    }
}
