using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xuan.UWP.Framework.Utils
{
    public class LogUtil
    {
        public static void Error(string tag, string message)
        {
            Output("Error", tag, message);
        }

        public static void Info(string tag, string message)
        {
            Output("Info", tag, message);
        }

        public static void Warn(string tag, string message)
        {
            Output("Warn", tag, message);

        }

        private static void Output(string type, string tag, string msg)
        {
            Debug.WriteLine($"{type}::{tag}:{msg}");
        }
    }
}
