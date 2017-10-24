using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xuan.UWP.Framework.Network {
    public class ResponseMessage<T> {
        public bool IsSuccess { get; set; }
        public ResponseCode Code { get; set; }
        public T Result { get; set; }
        public string ResultString { get; set; }
        public string Message { get; set; }
        public object Tag { get; set; }
    }

    public enum ResponseCode {
        /// <summary>
        /// 参数错误.
        /// </summary>
        ParamError = -1,
        /// <summary>
        /// 成功.
        /// </summary>
        Success = 0,
        /// <summary>
        /// 网络不可用.
        /// </summary>
        NetUnusual,
        /// <summary>
        /// 服务器返回异常.
        /// </summary>
        ServerError,
        /// <summary>
        /// 访问超时.
        /// </summary>
        Timeout,
        /// <summary>
        /// 用户请求被取消.
        /// </summary>
        UserCancel

    }
}
