using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xuan.UWP.Framework.ImageLib.Cache {
    public class SHA1CacheGenerator : ICacheGenerator {
        public string GeneratorName(string name) {
            return Xuan.UWP.Framework.Utils.SH1Util.ComputeHash(name);
        }
    }
}
