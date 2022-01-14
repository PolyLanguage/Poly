using System;
using System.Collections.Generic;
using System.Text;
using LLVMSharp;

using PolyToolkit.Parsing;
using PolyToolkit.Parsing.Ast;
namespace PolyToolkit.Compilation.LIR
{
    public class LIRGen
    {
        public PolyProgram Program { get; set; }
        public LIRGen(PolyProgram program)
        {
            Program = program;
        }

        public void Generate()
        {
            LLVMBuilderRef builder = LLVM.CreateBuilder();

            LLVM.LinkInMCJIT();
            LLVM.InitializeX86TargetInfo();
            LLVM.InitializeX86Target();
            LLVM.InitializeX86TargetMC();

            foreach(PolyCodefile file in Program.Files)
            {
                LLVMModuleRef module = GenerateModule(file.FileName.Replace(".poly", ""), file.CodeTree);
            }

            //TODO: return LIR
        }

        private LLVMModuleRef GenerateModule(string name,CodeTree code)
        {
            LLVMModuleRef module = LLVM.ModuleCreateWithName(name);

            // Create a function pass manager for this engine
            LLVMPassManagerRef passManager = LLVM.CreateFunctionPassManagerForModule(module);

            // Set up the optimizer pipeline.  Start with registering info about how the
            // target lays out data structures.
            // LLVM.DisposeTargetData(LLVM.GetExecutionEngineTargetData(engine));

            // Provide basic AliasAnalysis support for GVN.
            LLVM.AddBasicAliasAnalysisPass(passManager);
            // Promote allocas to registers.
            LLVM.AddPromoteMemoryToRegisterPass(passManager);
            // Do simple "peephole" optimizations and bit-twiddling optzns.
            LLVM.AddInstructionCombiningPass(passManager);
            // Reassociate expressions.
            LLVM.AddReassociatePass(passManager);
            // Eliminate Common SubExpressions.
            LLVM.AddGVNPass(passManager);
            // Simplify the control flow graph (deleting unreachable blocks, etc).
            LLVM.AddCFGSimplificationPass(passManager);
            LLVM.InitializeFunctionPassManager(passManager);

            //generate
            foreach(AstNode node in code.Childs)
            {
                VisitAndGenerate(node,module);
            }

            return module;
        }

        private void VisitAndGenerate(AstNode node,LLVMModuleRef module)
        {
            if(node is ClassNode)
            {
                //generate class

                //generate class members (method/field/constructor)
                foreach (AstNode clChild in ((ClassNode)node).Body.Childs)
                    VisitAndGenerate(node,module);
            }
        }
    }
}
