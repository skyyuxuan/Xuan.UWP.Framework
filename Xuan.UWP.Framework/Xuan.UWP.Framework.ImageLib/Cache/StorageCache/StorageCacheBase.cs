using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Xuan.UWP.Framework.ImageLib.Cache;

namespace Xuan.UWP.Framework.ImageLib.Cache {
    public abstract class StorageCacheBase {
        protected readonly StorageFolder _baseFolder;
        protected virtual string _cacheFolderName { get; set; }
        protected virtual ICacheGenerator _cacheFileNameGenerator { get; set; }
        public abstract Task<bool> SaveAsync(string url, IRandomAccessStream cacheStream);
        public abstract Task<bool> SaveAsync(StorageFile file);
        public abstract Task<IRandomAccessStream> GetAsync(string url);
        public abstract Task<StorageFile> GetFileAsync(string url);
        public abstract Task<bool> Remove(string url);


        protected StorageCacheBase(StorageFolder baseFolder, string cacheFolderName, ICacheGenerator cacheFileNameGenerator,
            long cacheMaxLifetime) {

            if (baseFolder == null) {
                throw new ArgumentNullException("folder");
            }

            if (string.IsNullOrEmpty(cacheFolderName)) {
                throw new ArgumentException("cacheFolderName name could not be null or empty");
            }

            if (cacheFolderName.StartsWith("\\")) {
                throw new ArgumentException("cacheFolderName name shouldn't starts with double slashes: \\");
            }

            if (cacheFileNameGenerator == null) {
                throw new ArgumentNullException("cacheFileNameGenerator");
            }

            _baseFolder = baseFolder;
            _cacheFolderName = cacheFolderName;
            _cacheFileNameGenerator = cacheFileNameGenerator;
        }

        public virtual string GetFullPath(string name) {
            return System.IO.Path.Combine(_cacheFolderName, name);
        }

        public virtual async Task<bool> IsCacheExists(string url) {
            var fullFilePath = GetFullPath(_cacheFileNameGenerator.GeneratorName(url));
            try {
                await _baseFolder.GetFileAsync(fullFilePath);
                return true;
            } catch {
                return false;
            }
        }
    }
}
