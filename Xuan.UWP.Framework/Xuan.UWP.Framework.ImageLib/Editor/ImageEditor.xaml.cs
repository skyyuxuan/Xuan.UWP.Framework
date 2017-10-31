using Microsoft.Graphics.Canvas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace Xuan.UWP.Framework.ImageLib.Editor {
    public sealed partial class ImageEditor : UserControl {

        public ImageEditor() {
            this.InitializeComponent();
        }

        private CanvasBitmap _imageBitmap;

        private void OnDraw(Microsoft.Graphics.Canvas.UI.Xaml.CanvasControl sender, Microsoft.Graphics.Canvas.UI.Xaml.CanvasDrawEventArgs args) {

        }

        private CanvasRenderTarget GetRenderTarget() {
            var width = canvasControl.ActualHeight;
            var height = canvasControl.ActualHeight;
            CanvasDevice device = CanvasDevice.GetSharedDevice();
            CanvasRenderTarget target = new CanvasRenderTarget(device, (float)width, (float)height, 96);
            using (CanvasDrawingSession graphics = target.CreateDrawingSession()) {

            }
            return target;
        }

        private void DrawImage() {
            var rec = GetImageDrawingRect();  
        }
        private Rect GetImageDrawingRect() {
            var rect = new Rect();
            return rect;
        }
    }
}
