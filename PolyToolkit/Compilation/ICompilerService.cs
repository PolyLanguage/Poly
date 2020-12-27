using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PolyToolkit.Compilation
{
    public interface ICompilerService
    {
        void CompileProgram(PolyProgram program);
        void CompileSingleFile(PolyCodefile file);

        string GenerateCode(PolyProgram program);
        byte[] GenerateByteCode(PolyProgram program);
    }
}
