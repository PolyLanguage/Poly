using System;
namespace PolyToolkit.VM
{
    public enum PBCInstructionType
    {
        /// <summary>
        /// Start of the code block
        /// </summary>
        BLOCK_BEGIN,
        /// <summary>
        /// End of the code block
        /// </summary>
        BLOCK_END,
        /// <summary>
        /// Jump to instruction
        /// </summary>
        JUMP,

        /// <summary>
        /// Set value for method argument
        /// </summary>
        SET_ARG,
        /// <summary>
        /// Call method
        /// </summary>
        CALL,
    }
}
