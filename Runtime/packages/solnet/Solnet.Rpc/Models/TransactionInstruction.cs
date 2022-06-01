using System.Collections.Generic;

namespace Solnet.Rpc.Models
{
    /// <summary>
    /// Represents a transaction instruction.
    /// </summary>
    /// 
    [System.Serializable]
    public class TransactionInstruction
    {
        /// <summary>
        /// The program ID associated with the instruction.
        /// </summary>
        public byte[] ProgramId { get; set; }
        
        /// <summary>
        /// The keys associated with the instruction.
        /// </summary>
        public IList<AccountMeta> Keys { get; set; }
        
        /// <summary>
        /// The instruction-specific data.
        /// </summary>
        public byte[] Data { get; set; }
    }

    [System.Serializable]
    public class TransactionInstructionForJS
    {
        /// <summary>
        /// The program ID associated with the instruction.
        /// </summary>
        public byte[] programId;

        /// <summary>
        /// The keys associated with the instruction.
        /// </summary>
        public List<AccountMetaForJS> keys;

        /// <summary>
        /// The instruction-specific data.
        /// </summary>
        public byte[] data;
    }
}