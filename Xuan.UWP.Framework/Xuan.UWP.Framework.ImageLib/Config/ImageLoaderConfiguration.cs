using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xuan.UWP.Framework.ImageLib.Cache;

namespace Xuan.UWP.Framework.ImageLib.Config {
    public class ImageLoaderConfiguration {
        public int ThreadPoolSize;
        public int ThreadPriority;
        public StorageCacheBase StorageCache;
        public DisplayImageOptions DisplayImageOptions;

        public class Builder {

            private ImageLoaderConfiguration config = new ImageLoaderConfiguration();

            public Builder ThreadPoolSize(int threadPoolSize) {
                config.ThreadPoolSize = threadPoolSize;
                return this;
            }

            public Builder ThreadPriority(int threadPriority) {
                config.ThreadPriority = threadPriority;
                return this;
            }


            public Builder StorageCache(StorageCacheBase storageCache) {
                config.StorageCache = storageCache;
                return this;
            }

            public Builder DefaultDisplayImageOptions(DisplayImageOptions displayImageOptions) {
                config.DisplayImageOptions = displayImageOptions;
                return this;
            }


            public ImageLoaderConfiguration Build() {
                return config;
            }
        }
    }
}
