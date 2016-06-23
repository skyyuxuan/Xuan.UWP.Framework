using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xuan.UWP.Framework.ImageLib.Cache.CacheGenerator
{
    public class SHA1CacheGenerator : ICacheGenerator
    {
        public string GeneratorName(string name)
        {
            return Utils.SH1Util.ComputeHash(name);
        }
    }
}
