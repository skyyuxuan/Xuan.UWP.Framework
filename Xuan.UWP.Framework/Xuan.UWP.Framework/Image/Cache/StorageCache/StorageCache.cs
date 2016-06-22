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

namespace Xuan.UWP.Framework.Image.Cache
{
    public class StorageCache : StorageCacheBase
    {
        ConcurrentDictionary<string, SemaphoreSlim> _dicConcurrentLocker = new ConcurrentDictionary<string, SemaphoreSlim>();

        public StorageCache(StorageFolder baseFolder, string cacheFolderName, ICacheGenerator cacheFileNameGenerator, long cacheMaxLifetime)
            : base(baseFolder, cacheFolderName, cacheFileNameGenerator, cacheMaxLifetime)
        {
        }

        public override Task<IRandomAccessStream> GetAsync(string url)
        {
            throw new NotImplementedException();
        }

        public override Task<StorageFile> GetFileAsync(string url)
        {
            throw new NotImplementedException();
        }

        public override Task<bool> Remove(string url)
        {
            throw new NotImplementedException();
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
                        await FileIO.WriteBufferAsync(storageFile, StreamUtil.RandomStreamToBuffer(cacheStream));
                        return true;
                    }
                }
                catch
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
