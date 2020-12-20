using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using PolyToolkit.Debug;
using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
namespace PolyToolkit.Compilation
{
    public class PolyCodefile
    {
        public string FilePath { get; }
        public string FileName { get; }
        public string FileBaseDir { get { return new FileInfo(FilePath).Directory.FullName; } }

        public string Content { get; private set; }
        public CodeTree CodeTree { get; private set; }

        public PolyCodefile(string filePath,string fileName)
        {
            FilePath = filePath;
            FileName = fileName;
        }

        public void ReadContent()
        {
            StreamReader sr = new StreamReader(FilePath);
            Content = sr.ReadToEnd();
            sr.Close();
        }
        public PolyLog ParseTree(bool stepLog=false,bool actionLog=false)
        {
            PolyParser parser = new PolyParser(Content);
            parser.DoStepLog = stepLog;
            parser.DoActionLog = actionLog;

            CodeTree = parser.ParseCode();

            return new PolyLog(parser.Log, parser.LexLog);
        }
    }
}
