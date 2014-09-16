using Creatalk;
using System;
using System.Collections;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Creatalk.UI.Controls
{
    public class MyRichTextBox : RichTextBox
    {
        private const string UrlPattern = "(http|ftp|https):\\/\\/[\\w\\-_]+(\\.[\\w\\-_]+)+([\\w\\-\\(?\\w+).,@?^=%&amp;:/~\\+#]*[\\w\\-\\@?^=%&amp;/~\\+#])?";
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(MyRichTextBox), new PropertyMetadata(default(string), TextPropertyChanged));

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        private static void TextPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            MyRichTextBox richTextBox = (MyRichTextBox)dependencyObject;
            string text = (string)dependencyPropertyChangedEventArgs.NewValue;
            int textPosition = 0;
            Paragraph paragraph = new Paragraph();

            MatchCollection urlMatches = Regex.Matches(text, UrlPattern);
            foreach (Match urlMatch in urlMatches)
            {
                int urlOccurrenceIndex = text.IndexOf(urlMatch.Value, textPosition, StringComparison.Ordinal);

                if (urlOccurrenceIndex != 0)
                {
                    paragraph.Inlines.Add(text.Substring(textPosition, urlOccurrenceIndex - textPosition));
                    textPosition = textPosition + (urlOccurrenceIndex - textPosition);

                    if (urlMatch.Value == "http://www.buhatayiboylefixliyorum.com/fixed.asp")
                    {
                        continue;
                    }

                    if (App.Session_LoadSession("aImage") == "1" && urlMatch.Value.IndexOf(".maxiresim") >= 0)
                    {
                        Image image = new Image
                        {
                            Source = new BitmapImage(new Uri(urlMatch.Value.Replace(".maxiresim", ""), UriKind.Absolute)),
                            Stretch = Stretch.None,
                            MaxWidth = 325
                        };

                        image.Tap += (s, e) =>
                        {
                            image.Stretch = Stretch.Uniform;
                        };

                        InlineUIContainer containter = new InlineUIContainer();
                        containter.Child = image;
                        paragraph.Inlines.Add(containter);
                    }
                    else
                    {
                        Hyperlink hyperlink = new Hyperlink
                        {
                            NavigateUri = new Uri(urlMatch.Value.Replace(".maxiresim", "")),
                            TargetName = "_blank",
                            Foreground = new SolidColorBrush(Colors.Blue)
                        };

                        hyperlink.Inlines.Add(urlMatch.Value.Replace(".maxiresim", ""));
                        paragraph.Inlines.Add(hyperlink);
                    }

                    textPosition += urlMatch.Value.Length;
                }
                else
                {
                    if (App.Session_LoadSession("aImage") == "1" && urlMatch.Value.IndexOf(".maxiresim") >= 0)
                    {
                        Image image = new Image
                        {
                            Source = new BitmapImage(new Uri(urlMatch.Value.Replace(".maxiresim", ""), UriKind.Absolute)),
                            Stretch = Stretch.None,
                            MaxWidth = 325
                        };

                        image.Tap += (s, e) =>
                        {
                            image.Stretch = Stretch.Uniform;
                        };

                        InlineUIContainer containter = new InlineUIContainer();
                        containter.Child = image;
                        paragraph.Inlines.Add(containter);
                    }
                    else
                    {
                        Hyperlink hyperlink = new Hyperlink
                        {
                            NavigateUri = new Uri(urlMatch.Value.Replace(".maxiresim", "")),
                            TargetName = "_blank",
                            Foreground = new SolidColorBrush(Colors.Blue)
                        };

                        hyperlink.Inlines.Add(urlMatch.Value.Replace(".maxiresim", ""));
                        paragraph.Inlines.Add(hyperlink);
                    }

                    textPosition += urlMatch.Value.Length;
                }
            }

            if (urlMatches.Count == 0)
            {
                paragraph.Inlines.Add(text);
            }

            richTextBox.Blocks.Clear();
            richTextBox.Blocks.Add(paragraph);
        }
    }
}
