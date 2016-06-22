using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xuan.UWP.Framework.Image
{
    public partial class ImageLoader
    {
        private static ImageLoader _instance;
        private static readonly object _lock = new object();
        private ImageLoader()
        { }

        public static ImageLoader GetInstance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {
                        if (_instance == null) _instance = new ImageLoader();
                    }
                }
                return _instance;
            }
        }
    }
}
