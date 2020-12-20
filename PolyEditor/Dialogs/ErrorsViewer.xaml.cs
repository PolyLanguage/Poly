using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Win = System.Windows;
using HandyControl.Data;
using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using System.IO;

using PolyToolkit;
using PolyToolkit.Debug;
using PolyToolkit.Parsing;
namespace PolyEditor
{
    public partial class ErrorsViewer : Window
    {
        public ErrorsViewer(PolyLog log)
        {
            InitializeComponent();

            //dark skin
            if(MainWindow.CurrentSkin == SkinType.Dark)
            {
                SharedResourceDictionary.SharedDictionaries.Clear();
                Resources.MergedDictionaries.Add(ResourceHelper.GetSkin(SkinType.Dark));
                Resources.MergedDictionaries.Add(new Win.ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
                });
                this?.OnApplyTemplate();
            }

            foreach(LexerError lexErr in log.LexerLog.Errors)
            {
                ErrsBox.Document = new Win.Documents.FlowDocument(new Win.Documents.Paragraph(
                    new Win.Documents.Run("[Lexer:Error]"+lexErr.Message)));
            }
            foreach(ParserError parErr in log.ParserLog.Errors)
            {
                ErrsBox.Document = new Win.Documents.FlowDocument(new Win.Documents.Paragraph(
                    new Win.Documents.Run("[Parser:Error]" + parErr.Message +
                    " (In "+parErr.ThrowedIn.ToString()+")")));
            }
        }
    }
}
