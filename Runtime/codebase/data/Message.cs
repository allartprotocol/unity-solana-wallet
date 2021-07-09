
﻿namespace AllArt.Solana
{
    using System.Collections.Generic;

    public class MessageHeader
    {
        /**
         * The number of signatures required for this message to be considered valid. The
         * signatures must match the first `numRequiredSignatures` of `accountKeys`.
         */
        public int numRequiredSignatures;
        /** The last `numReadonlySignedAccounts` of the signed keys are read-only accounts */
        public int numReadonlySignedAccounts;
        /** The last `numReadonlySignedAccounts` of the unsigned keys are read-only accounts */
        public int numReadonlyUnsignedAccounts;
    };

    public class MessageArgs
    {
        /** The message header, identifying signed and read-only `accountKeys` */
        public MessageHeader header;
        /** All the account keys used by this transaction */
        string[] accountKeys;
        /** The hash of a recent ledger block */
        public string recentBlockhash;
        /** Instructions that will be executed in sequence and committed in one atomic transaction if all succeed. */
        CompiledInstruction[] instructions;
    };

    public class Message
    {
        public MessageHeader header;
        public List<PublicKey> accountKeys;
        public string recentBlockhash;
        public CompiledInstruction[] instructions;
    };
}
