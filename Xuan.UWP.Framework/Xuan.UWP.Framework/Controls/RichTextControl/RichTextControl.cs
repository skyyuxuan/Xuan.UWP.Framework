using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

// The Templated Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234235

namespace Xuan.UWP.Framework.Controls
{
    [TemplatePart(Name = RichTextControl.SCROLLVIEWER, Type = typeof(ScrollViewer))]
    [TemplatePart(Name = RichTextControl.RICHTEXTBLOCK, Type = typeof(RichTextBlock))]

    public abstract class RichTextControl : Control
    {
        TaskCompletionSource<object> taskCompletionSource = new TaskCompletionSource<object>();

        private const string SCROLLVIEWER = "TC_Scroll";
        private const string RICHTEXTBLOCK = "TC_RichTextContent";

        private ScrollViewer scroll;
        private RichTextBlock richTextBlock;


        public RichTextControl()
        {
            this.DefaultStyleKey = typeof(RichTextControl);
        }

        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            scroll = GetTemplateChild(SCROLLVIEWER) as ScrollViewer;
            richTextBlock = GetTemplateChild(RICHTEXTBLOCK) as RichTextBlock;
            taskCompletionSource.SetResult(null);
        }


        public DataTemplate HeaderTemplate
        {
            get { return (DataTemplate)GetValue(HeaderTemplateProperty); }
            set { SetValue(HeaderTemplateProperty, value); }
        }

        public static readonly DependencyProperty HeaderTemplateProperty =
          DependencyProperty.Register(nameof(HeaderTemplate), typeof(DataTemplate), typeof(RichTextControl), new PropertyMetadata(null));

        public DataTemplate FootTemplate
        {
            get { return (DataTemplate)GetValue(FootTemplateProperty); }
            set { SetValue(FootTemplateProperty, value); }
        }

        public static readonly DependencyProperty FootTemplateProperty =
          DependencyProperty.Register(nameof(FootTemplate), typeof(DataTemplate), typeof(RichTextControl), new PropertyMetadata(null));
        public UIElement Header
        {
            get { return (UIElement)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(UIElement), typeof(RichTextControl), new PropertyMetadata(null));

        public UIElement Footer
        {
            get { return (UIElement)GetValue(FooterProperty); }
            set { SetValue(FooterProperty, value); }
        }

        public static readonly DependencyProperty FooterProperty =
            DependencyProperty.Register("Footer", typeof(UIElement), typeof(RichTextControl), new PropertyMetadata(null));

        public Brush ContentBackground
        {
            get { return (Brush)GetValue(ContentBackgroundProperty); }
            set { SetValue(ContentBackgroundProperty, value); }
        }

        public static readonly DependencyProperty ContentBackgroundProperty =
            DependencyProperty.Register("ContentBackground", typeof(Brush), typeof(RichTextControl), new PropertyMetadata(null));

        public double ContentMaxWidth
        {
            get { return (double)GetValue(ContentMaxWidthProperty); }
            set { SetValue(ContentMaxWidthProperty, value); }
        }

        public static readonly DependencyProperty ContentMaxWidthProperty =
            DependencyProperty.Register("ContentMaxWidth", typeof(double), typeof(RichTextControl), new PropertyMetadata(900d));

        public int LineHeight
        {
            get { return (int)GetValue(LineHeightProperty); }
            set { SetValue(LineHeightProperty, value); }
        }

        public static readonly DependencyProperty LineHeightProperty =
            DependencyProperty.Register("LineHeight", typeof(int), typeof(RichTextControl), new PropertyMetadata(20));


        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(RichTextControl), new PropertyMetadata(String.Empty, OnTextPropertyChanged));
        private static void OnTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as RichTextControl;
            var value = e.NewValue;
            if (value == null)
                return;
            sender.OnTextPropertyChanged(e.NewValue.ToString());
        }

        private async void OnTextPropertyChanged(string value)
        {
            if (!taskCompletionSource.Task.IsCompleted)
                await taskCompletionSource.Task;
            ChangeView(0, 0, 1);
            if (richTextBlock != null)
            {
                richTextBlock.Blocks.Clear();
                var paragraphs = CreateParagraph(value);
                foreach (var paragraph in paragraphs)
                {
                    richTextBlock.Blocks.Add(paragraph);
                } 
            }
        }

        public abstract IList<Paragraph> CreateParagraph(string text);

        public void ChangeView(Double? horizontalOffset, Double? verticalOffset, Single? zoomFactor, Boolean disableAnimation = true)
        {
            scroll?.ChangeView(horizontalOffset, verticalOffset, zoomFactor, disableAnimation);
        }
    }
}
