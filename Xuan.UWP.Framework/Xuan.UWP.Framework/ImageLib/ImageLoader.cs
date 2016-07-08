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

namespace Xuan.UWP.Framework.ImageLib
{

    public partial class ImageLoader
    {
        private TaskScheduler _sequentialScheduler;
        private static ImageLoader _instance;
        private static readonly object _lock = new object();


        private ImageLoader()
        { _sequentialScheduler = new LimitedConcurrencyLevelTaskScheduler(1, WorkItemPriority.Normal, true); }

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
                    randomStream = await LoadStreamFromCacheAsync(url, options);
                }
                if (randomStream == null)
                {
                    //TODO:need add download queue
                    RandomAccessStreamReference streamRef = RandomAccessStreamReference.CreateFromUri(new Uri(url));
                    randomStream = await streamRef.OpenReadAsync().AsTask(cancellationToken).ConfigureAwait(false);
                    await Task.Factory.StartNew(async () =>
                    {
                        await _config.StorageCache.SaveAsync(url, randomStream).ContinueWith(task =>
                         {
                             if (task.IsFaulted || !task.Result)
                             {

                             }
                         });
                    }, default(CancellationToken), TaskCreationOptions.AttachedToParent, _sequentialScheduler);
                }
            }
            return randomStream;
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
        protected virtual async Task<IRandomAccessStream> LoadStreamFromCacheAsync(string url, DisplayImageOptions options)
        {
            try
            {
                //文件存储中的缓存
                if (options.CacheOnStorage)
                {
                    if (await _config.StorageCache.IsCacheExists(url))
                    {
                        return await _config.StorageCache.GetAsync(url);
                    }
                }

            }
            catch (Exception e)
            {

            }
            return null;
        }

    }
}
