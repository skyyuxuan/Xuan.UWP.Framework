using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xuan.UWP.Framework.ImageLib.Config {
    public class DisplayImageOptions {
        public bool CacheOnStorage { get; private set; }
        public object Tag { get; private set; }

        public class Builder {
            DisplayImageOptions options = new DisplayImageOptions();

            public Builder CacheOnStorage(bool cacheOnDisk) {
                options.CacheOnStorage = cacheOnDisk;
                return this;
            }

            public Builder Tag(object tag) {
                options.Tag = tag;
                return this;
            }

            public DisplayImageOptions Build() {
                return options;
            }
        }
    }
}
