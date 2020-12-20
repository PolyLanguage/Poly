using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using Win = System.Windows;
using HandyControl.Data;
using HandyControl.Controls;
using HandyControl.Themes;
using HandyControl.Tools;
using System.IO;

using PolyToolkit;
using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
namespace PolyEditor
{
    public partial class MainWindow : Window
    {
        public static SkinType CurrentSkin { get; set; } = SkinType.Default;

        public MainWindow()
        {
            InitializeComponent();
            UpdateSkin(SkinType.Dark);
        }

        #region Menustrip Actions
        private void NewFile_Click(object sender, Win.RoutedEventArgs e)
        {
            SaveFileDialog dlg = new SaveFileDialog();
            dlg.Filter = "Poly Code File|*.poly|Any File|*.*";
            if (dlg.ShowDialog() == true)
            {
                WriteFile(dlg.FileName, "");
                TabItem tab = new TabItem();
                tab.Closed += delegate (object csender, EventArgs ce)
                {
                    TabItem ltab = (TabItem)csender;
                    WriteFile(((DocumentTag)ltab.Tag).DocumentLocation, ((ICSharpCode.AvalonEdit.TextEditor)ltab.Content).Text);
                };
                tab.Header = new FileInfo(dlg.FileName).Name;
                tab.Tag = new DocumentTag(dlg.FileName);
                tab.Content = CodeEditorFactory.CreateEditor("", CurrentSkin);
                TabsNav.Items.Add(tab);
                TabsNav.SelectedItem = tab;
            }
        }
        private void OpenFile_Click(object sender, Win.RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Filter = "Poly Code File|*.poly|Any File|*.*";
            if(dlg.ShowDialog() == true)
            {
                TabItem tab = new TabItem();
                tab.Closed += delegate(object csender,EventArgs ce)
                {
                    TabItem ltab = (TabItem)csender;
                    WriteFile(((DocumentTag)ltab.Tag).DocumentLocation, ((ICSharpCode.AvalonEdit.TextEditor)ltab.Content).Text);
                };
                tab.Header = new FileInfo(dlg.FileName).Name;
                tab.Tag = new DocumentTag(dlg.FileName);
                tab.Content = CodeEditorFactory.CreateEditor(File.ReadAllText(dlg.FileName), CurrentSkin);
                TabsNav.Items.Add(tab);
                TabsNav.SelectedItem = tab;
            }
        }
        private void SaveFile_Click(object sender, Win.RoutedEventArgs e)
        {
            TabItem tab = (TabItem)TabsNav.SelectedItem;
            if(tab != null)
                WriteFile(((DocumentTag)tab.Tag).DocumentLocation, ((ICSharpCode.AvalonEdit.TextEditor)tab.Content).Text);
        }
        private void SaveAsFile_Click(object sender, Win.RoutedEventArgs e)
        {
            TabItem tab = (TabItem)TabsNav.SelectedItem;
            if (tab != null)
            {
                SaveFileDialog dlg = new SaveFileDialog();
                dlg.Filter = "Poly Code File|*.poly|Any File|*.*";
                if (dlg.ShowDialog() == true)
                    WriteFile(dlg.FileName, ((ICSharpCode.AvalonEdit.TextEditor)tab.Content).Text);
                tab.Tag = new DocumentTag(dlg.FileName);
            }
        }
        private void SaveAllFile_Click(object sender, Win.RoutedEventArgs e)
        {
            foreach(TabItem tab in TabsNav.Items)
            {
                WriteFile(((DocumentTag)tab.Tag).DocumentLocation, ((ICSharpCode.AvalonEdit.TextEditor)tab.Content).Text);
            }
        }
        private void ViewAST_Click(object sender, Win.RoutedEventArgs e)
        {
            TabItem tab = (TabItem)TabsNav.SelectedItem;
            if (tab != null)
            {
                string code = ((ICSharpCode.AvalonEdit.TextEditor)tab.Content).Text;
                PolyParser parser = new PolyParser(code);
                CodeTree tree = parser.ParseCode();
                if(tree == null)
                {
                    try
                    {
                        new ErrorsViewer(new PolyToolkit.Debug.PolyLog(parser.Log, parser.LexLog)).ShowDialog();
                    }
                    catch { }
                }
                else
                    new SyntaxTreeViewer(tree).ShowDialog();
            }

        }

        private void DarkSkin_Click(object sender, Win.RoutedEventArgs e)
        {
            UpdateSkin(SkinType.Dark);
        }
        private void LightSkin_Click(object sender, Win.RoutedEventArgs e)
        {
            UpdateSkin(SkinType.Default);
        }
        #endregion

        #region Utils
        public void UpdateSkin(SkinType skin)
        {
            CurrentSkin = skin;

            SharedResourceDictionary.SharedDictionaries.Clear();
            Resources.MergedDictionaries.Add(ResourceHelper.GetSkin(skin));
            Resources.MergedDictionaries.Add(new Win.ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
            });
            this?.OnApplyTemplate();

            foreach (TabItem doc in TabsNav.Items)
                CodeEditorFactory.SetEditorSkin((ICSharpCode.AvalonEdit.TextEditor)doc.Content, skin);
        }
        public void WriteFile(string file,string content)
        {
            StreamWriter sw = new StreamWriter(file);
            sw.Write(content);
            sw.Close();
        }
        #endregion
    }
}
