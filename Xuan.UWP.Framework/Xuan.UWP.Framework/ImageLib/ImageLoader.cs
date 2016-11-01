using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources.Core;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.System.Threading;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;
using Xuan.UWP.Framework.ImageLib.Config;
using Xuan.UWP.Framework.Network;

namespace Xuan.UWP.Framework.ImageLib
{

    public partial class ImageLoader
    {
        private TaskScheduler _sequentialScheduler;
        private static ImageLoader _instance;
        private static readonly object _lock = new object();
        private object _concurrencyLock = new object();

        private Dictionary<int, ConcurrentTask<IRandomAccessStream>> _concurrentTasks = new Dictionary<int, ConcurrentTask<IRandomAccessStream>>();

        private class ConcurrentTask<T>
        {
            public Task<T> Task { get; set; }
        }
        private ImageLoader()
        {
            _sequentialScheduler = new LimitedConcurrencyLevelTaskScheduler(1, WorkItemPriority.Normal, true);
        }

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


        protected ImageLoaderConfiguration _config;
        public ImageLoader InitConfig(ImageLoaderConfiguration config)
        {
            this._config = config;
            return this;
        }

        public async Task<IRandomAccessStream> GetImageStreamAsync(string url, DisplayImageOptions options, CancellationToken cancellationToken)
        {
            CheckConfig();
            IRandomAccessStream randomStream = null;
            if (options == null)
            {
                options = new DisplayImageOptions.Builder()
                    .CacheOnStorage(true)
                    .Build();
            }
            if (string.IsNullOrEmpty(url))
            {

            }
            else
            {
                randomStream = await GetStreamFromUriAsync(new Uri(url), cancellationToken);
                if (randomStream == null)
                {
                    ConcurrentTask<IRandomAccessStream> requestTask = null;
                    int urlCode = url.GetHashCode();
                    lock (_concurrencyLock)
                    {
                        if (_concurrentTasks.ContainsKey(urlCode))
                        {
                            requestTask = _concurrentTasks[urlCode];
                        }
                    }
                    if (requestTask != null)
                    {
                        await requestTask.Task.ConfigureAwait(false);
                        requestTask = null;
                    }
                    if (requestTask == null)
                    {
                        requestTask = new ConcurrentTask<IRandomAccessStream>()
                        {
                            Task = GetStreamFromCacheOrNetAsync(url, options)
                        };

                        lock (_concurrencyLock)
                        {
                            if (_concurrentTasks.ContainsKey(urlCode))
                            {
                                _concurrentTasks.Remove(urlCode);
                            }
                            _concurrentTasks.Add(urlCode, requestTask);
                        }
                    }
                    try
                    {
                        randomStream = await requestTask.Task.ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        lock (_concurrencyLock)
                        {
                            if (_concurrentTasks.ContainsKey(urlCode))
                            {
                                _concurrentTasks.Remove(urlCode);
                            }
                        }
                        System.Diagnostics.Debug.WriteLine(ex.Message);
                    }
                    finally
                    {
                        lock (_concurrencyLock)
                        {
                            if (_concurrentTasks.ContainsKey(urlCode))
                            {
                                _concurrentTasks.Remove(urlCode);
                            }
                        }
                    }
                }
            }
            return randomStream;
        }

        public async Task<StorageFile> GetStorageFileFromCache(string url)
        {
            try
            {
                if (await _config.StorageCache.IsCacheExists(url))
                {
                    return await _config.StorageCache.GetFileAsync(url);
                }
                return null;
            }
            catch
            {
                throw new Exception("LoadImageStreamFromCache error");
            }
        }

        protected virtual void CheckConfig()
        {
            if (_config == null)
            {
                throw new InvalidOperationException("ImageLoader configuration was not setted, please Initialize ImageLoader instance with  ImageLoaderConfiguration");
            }
        }

        protected virtual async Task<IRandomAccessStream> GetStreamFromUriAsync(Uri uri, CancellationToken cancellationToken)
        {
            switch (uri.Scheme)
            {
                case "ms-appx":
                case "ms-appdata":
                    {
                        var file = await StorageFile.GetFileFromApplicationUriAsync(uri);
                        return await file.OpenAsync(FileAccessMode.Read).AsTask(cancellationToken).ConfigureAwait(false);
                    }
                case "ms-resource":
                    {
                        var rm = ResourceManager.Current;
                        var context = ResourceContext.GetForCurrentView();
                        var candidate = rm.MainResourceMap.GetValue(uri.LocalPath, context);
                        if (candidate != null && candidate.IsMatch)
                        {
                            var file = await candidate.GetValueAsFileAsync();
                            return await file.OpenAsync(FileAccessMode.Read).AsTask(cancellationToken).ConfigureAwait(false);
                        }
                        throw new Exception("Resource not found");
                    }
                case "file":
                    {
                        var file = await StorageFile.GetFileFromPathAsync(uri.LocalPath);
                        return await file.OpenAsync(FileAccessMode.Read).AsTask(cancellationToken).ConfigureAwait(false);
                    }
                default:
                    {
                        return null;
                    }
            }
        }
        protected virtual async Task<IRandomAccessStream> GetStreamFromCacheOrNetAsync(string url, DisplayImageOptions options)
        {
            IRandomAccessStream randomStream = null;
            randomStream = await GetStreamFromCacheAsync(url).ConfigureAwait(false);
            if (randomStream == null)
            {
                randomStream = await GetStreamFromNetAsync(url).ConfigureAwait(false);
                if (options.CacheOnStorage && randomStream != null && randomStream.Size > 0)
                {
                    await _config.StorageCache.SaveAsync(url, randomStream).ConfigureAwait(false);
                }
            }
            return randomStream;
        }
        protected virtual async Task<IRandomAccessStream> GetStreamFromCacheAsync(string url)
        {
            try
            {
                if (await _config.StorageCache.IsCacheExists(url))
                {
                    return await _config.StorageCache.GetAsync(url);
                }
            }
            catch (Exception e)
            {
                throw e;
            }
            return null;
        }
        protected virtual async Task<IRandomAccessStream> GetStreamFromNetAsync(string url)
        {
            IRandomAccessStream randomStream = null;
            using (var engine = new HttpEngine())
            {
                var request = new RequestMessage.Builder()
                    .Method("GET")
                    .Url(url)
                    .Build();
                using (var response = await engine.SendRequestAsync(request).ConfigureAwait(false))
                {
                    if (response != null)
                    {
                        randomStream = new InMemoryRandomAccessStream();
                        await response.Content.WriteToStreamAsync(randomStream).AsTask().ConfigureAwait(false);
                        randomStream.Seek(0);
                    }
                }

            }
            return randomStream;
        }
    }
}
