using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using LLVMSharp;
namespace PolyToolkit.Compilation.LIR
{
    public class LIRCompiler : ICompilerService
    {
        public PolyProgram Program { get; set; }

        public LIRCompiler(PolyProgram program)
        {
            Program = program;
        }

        #region Methods
        public void CompileProgram(PolyProgram program)
        {
            throw new NotImplementedException();
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
        #endregion
    }
}
