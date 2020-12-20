using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Reflection.Emit;

using PolyToolkit;
using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
namespace PolyToolkit.Compilation.IL
{
    public class ILCompiler : ICompilerService
    {
        public enum CompilerOutfile { Exe, Dll }

        public string CompilationPlatform { get; set; } = null;
        public CompilerOutfile OutType { get; set; } = CompilerOutfile.Exe;

        public void CompileProgram(PolyProgram program)
        {
            //filetype
            string fileExt = ".exe";
            if (OutType == CompilerOutfile.Dll)
                fileExt = ".dll";

            //assembly
            AssemblyName name = new AssemblyName(program.ProgramName);
            AssemblyBuilder assembly = AppDomain.CurrentDomain.DefineDynamicAssembly(
                name, AssemblyBuilderAccess.RunAndSave);
            ModuleBuilder md = assembly.DefineDynamicModule(name.Name, name.Name + fileExt);

            //read & parse files
            program.ReadAllFiles();
            program.ParseAllFiles();

            //define type from every type:
            //read every file
            foreach(PolyCodefile file in program.Files)
            {
                //check every node in root
                foreach(IAstNode node in file.CodeTree.Childs)
                {
                    //class
                    if(node is ClassNode)
                    {
                        ClassNode clNode = (ClassNode)node;

                        //define class
                        TypeBuilder tb = md.DefineType(clNode.Name.Value,
                            TypeAttributes.Public);
                        //define class childs
                        foreach(IAstNode clChild in clNode.Body.Childs)
                        {
                            if(clChild is VarDeclarationNode)
                            {
                                VarDeclarationNode varNode = (VarDeclarationNode)clChild;

                                FieldBuilder fb = tb.DefineField(varNode.VarName.Value, varNode.VarType.Type.ToNativeType(),
                                    FieldAttributes.Public);
                                
                            }
                            if(clChild is MethodNode)
                            {
                                MethodNode methNode = (MethodNode)clChild;

                                MethodBuilder mb = tb.DefineMethod(methNode.MethodName.Value,
                                    MethodAttributes.Public);
                                
                            }
                        }

                        tb.CreateType();
                    }
                }
            }

            //save
            assembly.Save(name.Name + fileExt);
        }

        public void CompileSingleFile(PolyCodefile file)
        {
            throw new NotImplementedException();
        }

        public string GenerateByteCode(PolyProgram program)
        {
            throw new NotImplementedException();
        }

        public string GenerateCode(PolyProgram program)
        {
            throw new NotImplementedException();
        }
    }
}
