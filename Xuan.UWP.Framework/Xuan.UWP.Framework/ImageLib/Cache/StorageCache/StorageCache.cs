using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Xuan.UWP.Framework.Utils;

namespace Xuan.UWP.Framework.ImageLib.Cache
{
    public class StorageCache : StorageCacheBase
    {
        ConcurrentDictionary<string, SemaphoreSlim> _dicConcurrentLocker = new ConcurrentDictionary<string, SemaphoreSlim>();

        public StorageCache(StorageFolder baseFolder, string cacheFolderName, ICacheGenerator cacheFileNameGenerator, long cacheMaxLifetime)
            : base(baseFolder, cacheFolderName, cacheFileNameGenerator, cacheMaxLifetime)
        {
        }

        public override async Task<IRandomAccessStream> GetAsync(string url)
        {
            var fullFilePath = GetFullPath(_cacheFileNameGenerator.GeneratorName(url));
            using (var ssFlile = _dicConcurrentLocker.GetOrAdd(fullFilePath, new SemaphoreSlim(1)))
            {
                try
                {
                    var cacheFileMemoryStream = new InMemoryRandomAccessStream();
                    var storageFile = await _baseFolder.GetFileAsync(fullFilePath);
                    using (var cacheFileStream = await storageFile.OpenAsync(FileAccessMode.Read))
                    {
                        await RandomAccessStream.CopyAsync(
                            cacheFileStream.GetInputStreamAt(0L),
                            cacheFileMemoryStream.GetOutputStreamAt(0L));
                        return cacheFileMemoryStream;
                    }
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex.StackTrace, ex.Message);
                    ssFlile.Release();
                    RemoveConcurrentLocker(fullFilePath);
                }
                finally
                {
                    ssFlile.Dispose();
                    RemoveConcurrentLocker(fullFilePath);
                }
                return null;
            }
        }

        public async override Task<StorageFile> GetFileAsync(string url)
        {
            var fullFilePath = GetFullPath(_cacheFileNameGenerator.GeneratorName(url));
            using (var ssFlile = _dicConcurrentLocker.GetOrAdd(fullFilePath, new SemaphoreSlim(1)))
            {
                try
                {
                    await ssFlile.WaitAsync();
                    return await _baseFolder.CreateFileAsync(fullFilePath, CreationCollisionOption.ReplaceExisting);
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex.StackTrace, ex.Message);
                    ssFlile.Release();
                    RemoveConcurrentLocker(fullFilePath);
                }
                finally
                {
                    ssFlile.Release();
                    RemoveConcurrentLocker(fullFilePath);
                }
                return null;
            }
        }

        public override async Task<bool> Remove(string url)
        {
            var fullFilePath = GetFullPath(_cacheFileNameGenerator.GeneratorName(url));
            using (var ssFlile = _dicConcurrentLocker.GetOrAdd(fullFilePath, new SemaphoreSlim(1)))
            {
                try
                {
                    await ssFlile.WaitAsync();
                    var file = await _baseFolder.TryGetItemAsync(fullFilePath);
                    if (file != null)
                    {
                        await file.DeleteAsync();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    LogUtil.Error(ex.StackTrace, ex.Message);
                    ssFlile.Release();
                    RemoveConcurrentLocker(fullFilePath);
                }
                finally
                {
                    ssFlile.Release();
                    RemoveConcurrentLocker(fullFilePath);
                }
                return false;
            }
        }

        public override async Task<bool> SaveAsync(string url, IRandomAccessStream cacheStream)
        { 
            var fullFilePath = GetFullPath(_cacheFileNameGenerator.GeneratorName(url));
            using (var ssFlile = _dicConcurrentLocker.GetOrAdd(fullFilePath, new SemaphoreSlim(1)))
            {
                try
                {
                    await ssFlile.WaitAsync();
                    var storageFile = await _baseFolder.CreateFileAsync(fullFilePath, CreationCollisionOption.ReplaceExisting);
                    if (storageFile != null)
                    {
                        cacheStream.Seek(0);
                        await FileIO.WriteBufferAsync(storageFile, StreamUtil.RandomStreamToBuffer(cacheStream));
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    ssFlile.Release();
                    RemoveConcurrentLocker(fullFilePath);
                }
                finally
                {
                    ssFlile.Release();
                    RemoveConcurrentLocker(fullFilePath);
                }
            }
            return false;

        }
        private void RemoveConcurrentLocker(string key)
        {
            SemaphoreSlim ssExists = null;
            _dicConcurrentLocker.TryRemove(key, out ssExists);
        }
    }
}
