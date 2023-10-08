using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using Windows.Web.Http.Filters;
using Xuan.UWP.Framework.Utils;

namespace Xuan.UWP.Framework.Network
{
    public class HttpEngine : IDisposable
    {
        private HttpClient _client = null;
        public HttpEngine()
             : this(null)
        {

        }
        public HttpEngine(IHttpFilter filter)
        {
            if (filter == null)
            {
                filter = new HttpBaseProtocolFilter() { AutomaticDecompression = true };
            }
            _client = new HttpClient(filter);
        }

        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationTokenSource cancellationTokenSource)
        {
            try
            {
                return await _client.SendRequestAsync(request).AsTask(cancellationTokenSource.Token);
            }
            catch (Exception e)
            {
                LogUtil.Error(e.StackTrace, e.Message);
                return null;
            }
        }
        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request)
        {
            try
            {
                return await _client.SendRequestAsync(request);
            }
            catch (Exception e)
            {
                LogUtil.Error(e.StackTrace, e.Message);
                return null;
            }
        }

        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, IProgress<HttpProgress> progress)
        {
            try
            {
                return await _client.SendRequestAsync(request).AsTask(progress);
            }
            catch (Exception e)
            {
                LogUtil.Error(e.StackTrace, e.Message);
                return null;
            }
        }
        public async Task<HttpResponseMessage> SendRequestAsync(HttpRequestMessage request, CancellationTokenSource cancellationTokenSource, IProgress<HttpProgress> progress)
        {
            try
            {
                return await _client.SendRequestAsync(request).AsTask(cancellationTokenSource.Token, progress);
            }
            catch (Exception e)
            {
                LogUtil.Error(e.StackTrace, e.Message);
                return null;
            }
        }

        public async Task<string> GetStringAsync(HttpRequestMessage request)
        {
            try
            {
                var response = await _client.SendRequestAsync(request);
                return await response?.Content.ReadAsStringAsync();
            }
            catch (Exception e)
            {
                LogUtil.Error(e.StackTrace, e.Message);
                return null;
            }
        }

        public async Task<IBuffer> GetBufferAsync(HttpRequestMessage request)
        {
            try
            {
                var response = await _client.SendRequestAsync(request);
                return await response?.Content.ReadAsBufferAsync();
            }
            catch (Exception e)
            {
                LogUtil.Error(e.StackTrace, e.Message);
                return null;
            }
        }

        public async Task<IInputStream> GetStreamAsync(HttpRequestMessage request)
        {
            try
            {
                var response = await _client.SendRequestAsync(request);
                return await response?.Content.ReadAsInputStreamAsync();
            }
            catch (Exception e)
            {
                LogUtil.Error(e.StackTrace, e.Message);
                return null;
            }
        }

        public void Dispose()
        {
            if (_client != null)
            {
                _client.Dispose();
                _client = null;
            }
        }
    }
}
