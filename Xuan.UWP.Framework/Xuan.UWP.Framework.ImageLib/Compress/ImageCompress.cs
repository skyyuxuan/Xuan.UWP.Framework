using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Graphics.Imaging;
using Windows.Storage;

namespace Xuan.UWP.Framework.ImageLib.Compress {
    public class ImageCompress {
        public async void Compress(StorageFile origFile, CompressOptions options) {

            using (var stream = await origFile.OpenReadAsync()) {
                var properties = await origFile.Properties.GetImagePropertiesAsync();
                uint width = properties.Width;
                uint height = properties.Height;

                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(stream);
                if (decoder == null) return;
                PixelDataProvider data = await decoder.GetPixelDataAsync();
                byte[] bytes = data.DetachPixelData();

                BitmapPropertySet propertySet = new BitmapPropertySet();
                BitmapTypedValue qualityValue = new BitmapTypedValue(options.Quality, PropertyType.Single);
                propertySet.Add("ImageQuality", qualityValue);

                BitmapEncoder be = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, stream, propertySet);
                be.SetPixelData(BitmapPixelFormat.Rgba8, BitmapAlphaMode.Straight, width, height, 96.0, 96.0, bytes);

                if (width > options.MaxWidth || height > options.MaxHeight) {
                    BitmapBounds bounds = new BitmapBounds();
                    if (width > options.MaxWidth) {
                        bounds.Width = options.MaxWidth;
                        bounds.X = (width - options.MaxWidth) / 2;
                    }
                    else bounds.Width = width;
                    if (height > options.MaxHeight) {
                        bounds.Height = options.MaxHeight;
                        bounds.Y = (height - options.MaxHeight) / 2;
                    }
                    else bounds.Height = height;
                    be.BitmapTransform.Bounds = bounds;
                }
                await be.FlushAsync();

            }
        }

    }
}
