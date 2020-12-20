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
using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
namespace PolyEditor
{
    public partial class SyntaxTreeViewer : Window
    {
        public SyntaxTreeViewer(CodeTree tree)
        {
            InitializeComponent();

            //dark skin
            if (MainWindow.CurrentSkin == SkinType.Dark)
            {
                SharedResourceDictionary.SharedDictionaries.Clear();
                Resources.MergedDictionaries.Add(ResourceHelper.GetSkin(SkinType.Dark));
                Resources.MergedDictionaries.Add(new Win.ResourceDictionary
                {
                    Source = new Uri("pack://application:,,,/HandyControl;component/Themes/Theme.xaml")
                });
                this?.OnApplyTemplate();
            }

            if (tree != null)
            {
                Win.Controls.TreeViewItem root = ProduceNode("Code");
                ProduceTree(root, tree);
                SyntaxTree.Items.Add(root);
            }
        }

        private void ProduceTree(Win.Controls.TreeViewItem tnode,IAstNode node)
        {
            foreach(IAstNode child in node.Childs)
            {
                if (child != null)
                {
                    Win.Controls.TreeViewItem tchild = ProduceNode(child);
                    ProduceTree(tchild, child);
                    tnode.Items.Add(tchild);
                }
            }
        }
        private Win.Controls.TreeViewItem ProduceNode(string header)
        {
            return new Win.Controls.TreeViewItem() { Header = header };
        }
        private Win.Controls.TreeViewItem ProduceNode(IAstNode node)
        {
            string header = node.GetType().Name;
            if (node is StaticStringNode)
                header += "(" +((StaticStringNode)node).Value + ")";
            else if(node is NameNode)
                header += "(" + ((NameNode)node).Value + ")";
            else if (node is TypenameNode)
                header += "(" + ((TypenameNode)node).Type.Name + ")";
            else if(node is IExpressionNode)
                header += "(" + ((IExpressionNode)node).Expression.Evaluate(new PolyToolkit.Evaluation.DefaultContext()) + ")";
            return new Win.Controls.TreeViewItem() { Header = header };
        }
    }
}
