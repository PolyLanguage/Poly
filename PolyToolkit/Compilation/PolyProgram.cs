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
    public class PolyProgram
    {
        public string ProgramName { get; set; }
        public string DirPath { get; set; }

        public List<PolyCodefile> Files { get; }

        public PolyProgram(string name,string outDir)
        {
            ProgramName = name;

            Files = new List<PolyCodefile>();
        }

        #region Compiler Actions
        public void CompileProgram<T>() where T : ICompilerService,new()
        {
            T compiler = new T();
            compiler.CompileProgram(this);
        }
        public string GenerateILCode<T>() where T : ICompilerService, new()
        {
            T compiler = new T();
            return compiler.GenerateCode(this);
        }
        public string GenerateByteCode<T>() where T : ICompilerService, new()
        {
            T compiler = new T();
            return compiler.GenerateByteCode(this);
        }
        #endregion

        #region Manage Files
        public void AddFile(string path,string name)
        {
            Files.Add(new PolyCodefile(path,name));
        }
        public void AddFilesFromDir(string dirpath)
        {
            //add files from dir
            foreach(string file in Directory.GetFiles(dirpath))
            {
                Files.Add(new PolyCodefile(file, file.Replace(dirpath, "")));
            }
            //add files from dirs in dir
            foreach(string dirIn in Directory.GetDirectories(dirpath))
            {
                string prepath = "";
                _AddFilesFromDir(prepath,dirpath);
            }
        }
        private void _AddFilesFromDir(string prepath,string dirpath)
        {
            //add files from dir
            foreach (string file in Directory.GetFiles(dirpath))
            {
                Files.Add(new PolyCodefile(file, prepath+file.Replace(dirpath, "")));
            }
        }
        #endregion

        #region Files Actions
        /// <summary>
        /// Reads all files contents
        /// </summary>
        public void ReadAllFiles()
        {
            foreach (PolyCodefile file in Files)
                file.ReadContent();
        }
        /// <summary>
        /// Reads specified file content
        /// </summary>
        /// <param name="name"></param>
        public void ReadFile(string filename)
        {
            bool success = false;
            foreach (PolyCodefile file in Files)
            {
                if (file.FileName == filename)
                {
                    file.ReadContent();
                    success = true;
                }
            }

            if(success==false)
                throw new Exception("File with that name not found");
        }
        /// <summary>
        /// Parses all files codetrees and returns log
        /// </summary>
        /// <returns>log</returns>
        public string[] ParseAllFiles(bool stepLog=false,bool actLog=false)
        {
            List<string> log = new List<string>();
            foreach (PolyCodefile file in Files)
            {
                PolyLog plog = file.ParseTree(stepLog,actLog);
                foreach (ParserError err in plog.ParserLog.Errors)
                    log.Add(file.FileName + ":" + err.ToString());
                foreach(LexerError err in plog.LexerLog.Errors)
                    log.Add(file.FileName + ":" + err.ToString());
            }
            return log.ToArray();
        }
        /// <summary>
        /// Parses specified file codetree and returns log
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string[] ParseFile(string filename,
            bool stepLog = false, bool actLog = false)
        {
            foreach (PolyCodefile file in Files)
            {
                if (file.FileName == filename)
                {
                    PolyLog plog = file.ParseTree(stepLog,actLog);
                    List<string> log = new List<string>();
                    foreach (ParserError err in plog.ParserLog.Errors)
                        log.Add(file.FileName + ":" + err.ToString());
                    foreach (LexerError err in plog.LexerLog.Errors)
                        log.Add(file.FileName + ":" + err.ToString());
                    return log.ToArray();
                }
            }
            throw new Exception("File with that name not found");
        }
        /// <summary>
        /// Gets all codetrees from all files
        /// </summary>
        /// <returns></returns>
        public CodeTree[] GetAllTrees()
        {
            List<CodeTree> trees = new List<CodeTree>();
            foreach (PolyCodefile file in Files)
                trees.Add(file.CodeTree);
            return trees.ToArray();
        }
        /// <summary>
        /// Gets codetree of specified file
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public CodeTree GetFileTree(string filename)
        {
            foreach(PolyCodefile file in Files)
            {
                if (file.FileName == filename)
                    return file.CodeTree;
            }
            throw new Exception("File with that name not found");
        }
        #endregion
    }
}
