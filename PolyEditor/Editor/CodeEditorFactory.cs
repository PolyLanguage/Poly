using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win = System.Windows;
using IO = System.IO;
using XML = System.Xml;
using HandyControl.Controls;

using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Highlighting;
namespace PolyEditor
{
    public static class CodeEditorFactory
    {
        public static TextEditor CreateEditor(string content,HandyControl.Data.SkinType theme)
        {
            TextEditor editor = new TextEditor();
            editor.VerticalAlignment = Win.VerticalAlignment.Stretch;
            editor.HorizontalAlignment = Win.HorizontalAlignment.Stretch;
            editor.ShowLineNumbers = true;
            editor.Text = content;
            editor.FontSize = 20;
            editor.PreviewMouseWheel += TextEditor_PreviewMouseWheel;

            SetEditorSkin(editor, theme);
            return editor;
        }
        public static void SetEditorSkin(TextEditor editor, HandyControl.Data.SkinType skin)
        {
            //light
            if (skin == HandyControl.Data.SkinType.Default || skin == HandyControl.Data.SkinType.Violet)
            {
                //theme
                editor.LineNumbersForeground =
                    new Win.Media.SolidColorBrush(Win.Media.Color.FromRgb(100,100,100));
                editor.Background =
                    new Win.Media.SolidColorBrush(Win.Media.Color.FromRgb(250,250,250));
                editor.Foreground =
                    new Win.Media.SolidColorBrush(Win.Media.Color.FromRgb(12,12,12));

                //syntax
                using (IO.Stream stream = Win.Application.GetResourceStream(new Uri(
                        "pack://application:,,/Editor/LightSyntax.xshd", UriKind.RelativeOrAbsolute)).Stream)
                {
                    using (XML.XmlTextReader reader = new XML.XmlTextReader(stream))
                    {
                        editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
            //dark
            else
            {
                //theme
                editor.LineNumbersForeground =
                    new Win.Media.SolidColorBrush(Win.Media.Color.FromRgb(43, 145, 175));
                editor.Background =
                    new Win.Media.SolidColorBrush(Win.Media.Color.FromRgb(30, 30, 30));
                editor.Foreground =
                    new Win.Media.SolidColorBrush(Win.Media.Color.FromRgb(220, 220, 220));

                //syntax
                using (IO.Stream stream = Win.Application.GetResourceStream(new Uri(
                        "pack://application:,,/Editor/DarkSyntax.xshd", UriKind.RelativeOrAbsolute)).Stream)
                {
                    using (XML.XmlTextReader reader = new XML.XmlTextReader(stream))
                    {
                        editor.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader, HighlightingManager.Instance);
                    }
                }
            }
        }
        #region Zoom
        private static void TextEditor_PreviewMouseWheel(object sender, Win.Input.MouseWheelEventArgs e)
        {
            bool ctrl = Win.Input.Keyboard.Modifiers == Win.Input.ModifierKeys.Control;
            if (ctrl)
            {
                UpdateFontSize(e.Delta > 0,(TextEditor)sender);
                e.Handled = true;
            }
        }
        // Reasonable max and min font size values
        private const double FONT_MAX_SIZE = 60d;
        private const double FONT_MIN_SIZE = 5d;

        // Update function, increases/decreases by a specific increment
        public static void UpdateFontSize(bool increase,TextEditor sender)
        {
            double currentSize = sender.FontSize;

            if (increase)
            {
                if (currentSize < FONT_MAX_SIZE)
                {
                    double newSize = Math.Min(FONT_MAX_SIZE, currentSize + 1);
                    sender.FontSize = newSize;
                }
            }
            else
            {
                if (currentSize > FONT_MIN_SIZE)
                {
                    double newSize = Math.Max(FONT_MIN_SIZE, currentSize - 1);
                    sender.FontSize = newSize;
                }
            }
        }
        #endregion
    }
}
